using System;
using System.Collections.Generic;
using System.Linq;
using SpookyCore.Runtime.EntitySystem;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    [CreateAssetMenu(menuName = "SpookyCore/Systems/Global Asset Registry/Entity Prefab Registry", fileName = "EntityPrefabRegistry")]
    public class EntityPrefabRegistry : ScriptableObject
    {
        [Serializable]
        public class PrefabEntry
        {
            public EntityID Key;
            public GameObject Prefab;
        }

        public List<PrefabEntry> Prefabs;

        private Dictionary<EntityID, GameObject> _lookup;

        public GameObject Get(EntityID key)
        {
            _lookup ??= Prefabs.ToDictionary(p => p.Key, p => p.Prefab);
            return _lookup.GetValueOrDefault(key);
        }

        // public T GetComponent<T>(EntityID key) where T : EntityComponent
        // {
        //     var prefab = Get(key);
        //     return prefab ? prefab.GetComponent<T>() : null;
        // }
    }

}