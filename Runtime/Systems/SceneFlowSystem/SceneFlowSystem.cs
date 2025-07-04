using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpookyCore.Runtime.Systems
{
    public class SceneFlowSystem : PersistentMonoSingleton<SceneFlowSystem>, IBootstrapSystem
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

        public async Task ReloadCurrentScene()
        {
            if (_isLoadingScene)
                return;

            _isLoadingScene = true;

            var oldScene = SceneManager.GetActiveScene();
            _previousScene = _currentScene;

            OnSceneEnded?.Invoke(_previousScene);

            var sceneIndex = _sceneConfig.GetSceneBuildIndex(_currentScene);
            var loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            await AwaitSceneLoad(loadOp);

            var newScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            SceneManager.SetActiveScene(newScene);
            OnSceneStarted?.Invoke(_currentScene);

            // Delay unload for safety in singletons/scripts execution order
            await Task.Yield();
            var unloadOp = SceneManager.UnloadSceneAsync(oldScene);
            await AwaitSceneUnload(unloadOp);

            _isLoadingScene = false;
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

            var sceneIndex = _sceneConfig.GetSceneBuildIndex(targetScene);
            var loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            await AwaitSceneLoad(loadOp);

            var newScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            SceneManager.SetActiveScene(newScene);
            OnSceneStarted?.Invoke(targetScene);

            // Delay unload for safety in singletons/scripts execution order
            await Task.Yield();
            var unloadOp = SceneManager.UnloadSceneAsync(oldScene);
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
            while (op is { isDone: false })
                await Task.Yield();
        }

        public Task OnBootstrapAsync(BootstrapContext context)
        {
            return Task.CompletedTask;
        }
    }
}