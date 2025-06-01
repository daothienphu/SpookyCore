using UnityEngine;

namespace SpookyCore.Utilities
{
    public abstract class MonoSingleton<T> : StaticMonoInstance<T> where T: MonoBehaviour{
        protected override void OnAwake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            base.OnAwake();
        }
    }
}