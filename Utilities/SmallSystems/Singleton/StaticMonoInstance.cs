using UnityEngine;

namespace SpookyCore.Utilities
{
    public abstract class StaticMonoInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        private void Awake() => OnAwake();
        private void Start() => OnStart();
        private void Update() => OnUpdate();
        private void FixedUpdate() => OnFixedUpdate();
        private void LateUpdate() => OnLateUpdate();

        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        protected virtual void OnLateUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnStart() { }
        protected virtual void OnAwake() => Instance = this as T;
    
        protected virtual void OnApplicationQuit()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}