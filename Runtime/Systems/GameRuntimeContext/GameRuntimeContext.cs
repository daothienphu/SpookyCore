using System.Threading.Tasks;
using SpookyCore.Runtime.EntitySystem;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    public class GameRuntimeContext : MonoSingleton<GameRuntimeContext>, IBootstrapSystem
    {
        #region Fields
        
        public Entity PlayerEntity { get; private set; }
        public Camera MainCamera { get; private set; }

        #endregion

        #region Life Cycle

        public Task OnBootstrapAsync(BootstrapContext context)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Public Methods

        public void RegisterPlayer(Entity player)
        {
            PlayerEntity = player;
        }

        public void SetMainCamera(Camera camera)
        {
            MainCamera = camera;
        }

        public void ResetContext()
        {
            PlayerEntity = null;
            MainCamera = null;
        }

        #endregion
    }
}