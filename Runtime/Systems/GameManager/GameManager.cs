using System.Threading.Tasks;
using SpookyCore.Runtime.EntitySystem;
using SpookyCore.Runtime.UI;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    public class GameManager : PersistentMonoSingleton<GameManager>, IBootstrapSystem
    {
        #region Fields

        [field: SerializeField] public Observable<Entity> PlayerObservable { get; private set; }
        [field: SerializeField] public int EnemyKilled { get; set; }
        [SerializeField] private Camera _mainCamera;
        public Camera MainCamera 
        {  
            get
            {
                if (_mainCamera) return _mainCamera;
                
                _mainCamera = Camera.main;
                if (!_mainCamera)
                {
                    Debug.LogWarning("<color=yellow>[Game Manager]</color> Main camera not found in scene.");
                }

                return _mainCamera;
            }
            private set => _mainCamera = value;
        }

        #endregion

        #region Life Cycle
        
        public Task OnBootstrapAsync(BootstrapContext context)
        {
            PlayerObservable = new Observable<Entity>(null, bypassValueChangeCheck: true);
            
            Debug.Log("<color=cyan>[Game Manager]</color> ready.");
            return Task.CompletedTask;
        }

        #endregion

        #region Public Methods

        public void RegisterPlayer(Entity player)
        {
            PlayerObservable.Value = player;
        }

        public void SetMainCamera(Camera camera)
        {
            MainCamera = camera;
        }

        public void ResetContext()
        {
            PlayerObservable = null;
            MainCamera = null;
        }

        #endregion
    }
}