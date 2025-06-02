using System;
using System.Collections.Generic;
using SpookyCore.Runtime.EntitySystem;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    public class PoolSystem : MonoSingleton<PoolSystem>
    {
        [SerializeField] private PoolObjectsConfig _poolConfig;
        private Dictionary<EntityID, ObjectPool<Entity>> _pools = new();

        protected override void OnAwake()
        {
            base.OnAwake();
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var entry in _poolConfig.PoolEntries)
            {
                if (GlobalAssetRegistry.Instance.TryGetPrefab(entry.ID, out var prefab))
                {
                    CreatePool(entry.ID, prefab, entry.InitialSize);
                }
                else
                {
                    Debug.Log($"Cannot create pool for entity {entry.ID}");
                }
            }

            Debug.Log("<color=cyan>[Pool System]</color> ready.");
        }

        public void CreatePool(EntityID id, GameObject prefab, int size)
        {
            if (!_pools.ContainsKey(id))
            {
                var parentGO = new GameObject($"{id}");
                _pools[id] = new ObjectPool<Entity>(prefab, size, parentGO.transform);
            }
        }

        /// <summary>
        /// OnGetCallback is called before the entity's OnAwake.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onGetCallback"></param>
        /// <param name="getPreviewVersion"> Get the preview version of an entity, the preview version will stop its life cycle after OnAwake</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(EntityID id, Action<Entity> onGetCallback = null, bool getPreviewVersion = false) where T : Entity
        {
            if (!_pools.TryGetValue(id, out var pool))
            {
                Debug.Log($"<color=cyan>[Pool System]</color> Pool for {id} does not exist, creating pool.");
                if (GlobalAssetRegistry.Instance.TryGetPrefab(id, out var prefab))
                {
                    CreatePool(id, prefab, 10);
                    pool = _pools[id];
                }
                else
                {
                    Debug.Log($"<color=cyan>[Pool System]</color> Cannot create pool for entity {id}.");
                }
            }
            
            return pool?.Get(onGetCallback: onGetCallback, getPreviewVersion: getPreviewVersion) as T;
        }

        public void Return<T>(EntityID id, T obj) where T : Entity
        {
            if (_pools.TryGetValue(id, out var pool))
            {
                pool.Return(obj);
            }
            else
            {
                Debug.LogError($"<color=cyan>[Pool System]</color> No pools exist for EntityID {id}. Destroying object.");
                Destroy(obj.gameObject);
            }
        }
    }
}