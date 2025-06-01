using System;
using System.Collections.Generic;
using SpookyCore.EntitySystem;
using UnityEngine;

namespace SpookyCore.SystemLoader.SubSystems.EntityDatabase
{
    [CreateAssetMenu(menuName = "SpookyCore/System Configs/Entity Database/Entity Prefabs Config", fileName = "Entity_Prefabs_Config")]
    public class EntityPrefabsConfig : ScriptableObject
    {
        [Serializable]
        public struct EntityPrefabEntry
        {
            public EntityID ID;
            public GameObject Prefab;
        }
        
        public List<EntityPrefabEntry> Prefabs;

        public bool TryGetPrefab(EntityID id, out GameObject prefab)
        {
            foreach (var entry in Prefabs)
            {
                if (entry.ID == id)
                {
                    prefab = entry.Prefab;
                    return true;
                }
            }
            
            Debug.LogError($"Entity {id} not found in database.");
            prefab = null;
            return false;
        }
    }
}