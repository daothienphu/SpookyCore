using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    public abstract class MonoSingleton<T> : StaticMonoInstance<T> where T: MonoBehaviour
    {
        private static object _lock = new();

        public new static T Instance
        {
            get
            {
                if (StaticMonoInstance<T>.Instance)
                {
                    return StaticMonoInstance<T>.Instance;
                }

                lock (_lock)
                {
                    var found = FindFirstObjectByType<T>();

                    if (found)
                    {
                        StaticMonoInstance<T>.Instance = found;
                        return found;
                    }
                    
                    Debug.LogWarning($"<color=cyan>{typeof(T).Name}</color> not found in scene.");
                    return null;
                }
            }
        }
        
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