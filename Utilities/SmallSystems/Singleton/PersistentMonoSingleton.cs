using UnityEngine;

namespace SpookyCore.Utilities
{
    public abstract class PersistentMonoSingleton<T> : MonoSingleton<T> where T: MonoBehaviour {
        protected override void OnAwake() {
            base.OnAwake();
            DontDestroyOnLoad(gameObject);
        }
    }
}