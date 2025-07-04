using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    [CreateAssetMenu(menuName = "SpookyCore/Systems/Bootstrap System/System Registry", fileName = "SystemRegistry")]
    public class SystemRegistry : ScriptableObject
    {
        [Serializable]
        public class PrefabEntry
        {
            public string SystemTypeName;
            public GameObject Prefab;
        }

        public List<PrefabEntry> Prefabs;

        public GameObject GetPrefabForType(Type systemType)
        {
            var entry = Prefabs.FirstOrDefault(e => e.SystemTypeName == systemType.Name);
            return entry?.Prefab;
        }
        
#if UNITY_EDITOR

        private void OnValidate()
        {
            foreach (var entry in Prefabs)
            {
                var component = entry.Prefab.GetComponent<IBootstrapSystem>();
                entry.SystemTypeName = component.GetType().Name;
            }
        }

#endif
    }
}