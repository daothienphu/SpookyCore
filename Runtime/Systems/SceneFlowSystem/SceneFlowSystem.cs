using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpookyCore.Runtime.Systems
{
    public class SceneFlowSystem : PersistentMonoSingleton<SceneFlowSystem>
    {
        [SerializeField] private SceneFlowConfig _sceneConfig;

        private bool _isLoadingScene;
        private SceneID _currentScene;
        private SceneID _previousScene;

        public event Action<SceneID> OnSceneStarted;
        public event Action<SceneID> OnSceneEnded;

        public SceneID CurrentScene => _currentScene;
        public SceneID PreviousScene => _previousScene;
        public bool IsTransitioning => _isLoadingScene;

        protected override void OnStart()
        {
            base.OnStart();
            _currentScene = SceneManager.GetActiveScene().ToSceneID();
        }

        public async Task SwitchToSceneAsync(SceneID targetScene)
        {
            if (_isLoadingScene || _currentScene == targetScene)
                return;

            _isLoadingScene = true;

            var oldScene = SceneManager.GetActiveScene();
            _previousScene = _currentScene;
            _currentScene = targetScene;

            OnSceneEnded?.Invoke(_previousScene);

            int sceneIndex = _sceneConfig.GetSceneBuildIndex(targetScene);
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            await AwaitSceneLoad(loadOp);

            var newScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            SceneManager.SetActiveScene(newScene);
            OnSceneStarted?.Invoke(targetScene);

            // Delay unload for safety in singletons/scripts execution order
            await Task.Yield();
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(oldScene);
            await AwaitSceneUnload(unloadOp);

            _isLoadingScene = false;
        }

        private static async Task AwaitSceneLoad(AsyncOperation op)
        {
            while (!op.isDone)
                await Task.Yield();
        }

        private static async Task AwaitSceneUnload(AsyncOperation op)
        {
            while (op != null && !op.isDone)
                await Task.Yield();
        }
    }
}