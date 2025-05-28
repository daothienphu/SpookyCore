using System.Collections.Generic;
using SpookyCore.EntitySystem;
using SpookyCore.SystemLoader.Event;
using SpookyCore.Systems.ObjectiveSystem;
using SpookyCore.UISystem;
using SpookyCore.Utilities;
using SpookyCore.Utilities.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpookyCore.SystemLoader
{
    public class GameSessionData : PersistentMonoSingleton<GameSessionData>, IGameSystem
    {
        #region Structs & Enums

        public enum GameScene
        {
            Bootstrap,
            MainMenu,
            Pregame,
            Gameplay,
            UITest,
            
        }

        private Dictionary<GameScene, int> _sceneIdxCache = new()
        {
            { GameScene.Bootstrap, 0 },
            { GameScene.MainMenu, 1 },
            { GameScene.Pregame, 2 },
            { GameScene.Gameplay, 3 },
            { GameScene.UITest, 4 },
        };

        #endregion
        
        #region Fields
        
        private GameScene _currentScene = GameScene.Bootstrap;
        private static int _pendingPreviousScene;
        private IEventBus _eventBus;
        private ObjectiveSystem _objectiveSystem;
        private static AsyncOperation _loadOperation;
        private static AsyncOperation _unloadOperation;
        
        #endregion

        #region Properties

        public Observable<Entity> ObservablePlayer { get; private set; }
        public Camera MainCamera { get; private set; }
        public GameData GameData { get; private set; }
        
        #endregion

        #region Life Cycle

        protected override void OnStart()
        {
            base.OnStart();

            _objectiveSystem = ObjectiveSystem.Instance;
            
            GameData = SaveLoadSystem.Load<GameData>();
            
            MainCamera = Camera.main;

            _eventBus = ServiceLocator.Instance.Get<IEventBus>();
            _eventBus.Subscribe<StartSavingGameProgress>(OnGameProgressSaved);
            _eventBus.Subscribe<StartLoadingGameProgress>(OnGameProgressLoaded);
            _eventBus.Subscribe<GameplayEndedEvent>(OnGameEnded);
            
            InitObservables();
            Debug.Log("<color=cyan>[Game Session Data]</color> system ready.");
        }

        private void OnEnable()
        {
            if (_eventBus == null) return;
            _eventBus.Subscribe<StartSavingGameProgress>(OnGameProgressSaved);
            _eventBus.Subscribe<StartLoadingGameProgress>(OnGameProgressLoaded);
            _eventBus.Subscribe<GameplayEndedEvent>(OnGameEnded);
        }

        private void OnDisable()
        {
            if (_eventBus == null) return;
            _eventBus.Unsubscribe<StartSavingGameProgress>(OnGameProgressSaved);
            _eventBus.Unsubscribe<StartLoadingGameProgress>(OnGameProgressLoaded);
            _eventBus.Unsubscribe<GameplayEndedEvent>(OnGameEnded);
        }

        private void OnGameProgressSaved(StartSavingGameProgress _)
        {
            //Save Objectives
            if (!_objectiveSystem) _objectiveSystem = ObjectiveSystem.Instance;
            var objectiveData = _objectiveSystem.SaveObjectiveProgress();
            GameData.PlayerData.Objectives.Clear();
            GameData.PlayerData.Objectives.AddRange(objectiveData);
            
            //Save inventory
            ObservablePlayer.TryInvoke(player =>
            {
                var playerData = player.SaveGameData();
                if (playerData == null)
                {
                    return;
                }
                
                GameData.PlayerData.Inventory.Clear();
                GameData.PlayerData.Inventory.AddRange(playerData.Inventory);
            });
            
            SaveLoadSystem.Save(GameData);
        }

        private void OnGameProgressLoaded(StartLoadingGameProgress _)
        {
            GameData = SaveLoadSystem.Load<GameData>();
            
            //Load Inventory
            ObservablePlayer.TryInvoke(player => player.LoadGameData(GameData.PlayerData));
            
            //Load Objectives
            if (!_objectiveSystem) _objectiveSystem = ObjectiveSystem.Instance;
            _objectiveSystem?.LoadObjectiveProgress(GameData.PlayerData.Objectives);
        }

        private void OnGameEnded(GameplayEndedEvent _)
        {
            
        }

        #endregion
        
        #region Public Methods

        public void AssignPlayer(Entity player)
        {
            ObservablePlayer.Value = player;
        }

        public void SwitchScene(GameScene scene)
        {
            if (_currentScene == scene) return;

            _currentScene = scene;
            LoadScene(_sceneIdxCache[_currentScene]);
        }
        
        public void SwitchToMainMenu()
        {
            if (_currentScene == GameScene.MainMenu) return;

            _currentScene = GameScene.MainMenu;
            LoadScene(_sceneIdxCache[_currentScene]);
        }

        public void SwitchToUITest()
        {
            if (_currentScene == GameScene.UITest) return;
            _currentScene = GameScene.UITest;
            LoadScene(_sceneIdxCache[_currentScene]);
        }
        
        public void SwitchToGameplay()
        {
            if (_currentScene == GameScene.Gameplay) return;

            _currentScene = GameScene.Gameplay;
            LoadScene(_sceneIdxCache[_currentScene]);
        }

        public void SwitchToPregame()
        {
            if (_currentScene == GameScene.Pregame) return;
            
            _currentScene = GameScene.Pregame;
            LoadScene(_sceneIdxCache[_currentScene]);
        }

        #endregion

        #region Private Methods
        
        private static void LoadScene(int sceneIdx)
        {
            _pendingPreviousScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.sceneLoaded += ActivatorAndUnloader;
            SceneManager.LoadScene(sceneIdx, LoadSceneMode.Additive);
        }
        
        private static void ActivatorAndUnloader(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= ActivatorAndUnloader;
            SceneManager.SetActiveScene(scene);
            SceneManager.UnloadSceneAsync(_pendingPreviousScene);
        }
        
        private void InitObservables()
        {
            ObservablePlayer = new Observable<Entity>(null, true);
        }

        #endregion
    }
}