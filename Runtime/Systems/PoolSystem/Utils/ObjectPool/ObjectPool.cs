using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace SpookyCore.Runtime.Systems
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        #region Fields

        private readonly Queue<T> _pool = new();
        private readonly GameObject _prefab;
        private readonly Transform _parent;

        #endregion

        #region Public Methods

        public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            for (var i = 0; i < initialSize; i++)
            {
                AddToPool();
            }
        }
        
        public T Get(Action<T> onGetCallback = null, bool resetParent = false, bool getPreviewVersion = false)
        {
            if (_pool.Count == 0)
            {
                AddToPool();
            }

            var obj = _pool.Dequeue();
            if (resetParent)
            {
                obj.transform.SetParent(null);
                obj.transform.SetParent(_parent);
            }
            obj.gameObject.SetActive(true);
            obj.GetComponent<IPoolable>()?.OnGettingFromPool(getPreviewVersion);
            onGetCallback?.Invoke(obj);
            return obj;
        }

        public void Return(T obj, Action<T> onReturnCallback = null)
        {
            obj.GetComponent<IPoolable>()?.OnReturningToPool();
            onReturnCallback?.Invoke(obj);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }

        #endregion

        #region Private Methods

        private void AddToPool()
        {
            var obj = Object.Instantiate(_prefab, _parent, false);
            obj.SetActive(false);

            if (obj.TryGetComponent(out T component))
            {
                _pool.Enqueue(component);
            }
            else
            {
                Debug.LogError($"Prefab {_prefab.name} does not contain a component of type {typeof(T)}");
                Object.Destroy(obj);
            }
        }

        #endregion
    }
}