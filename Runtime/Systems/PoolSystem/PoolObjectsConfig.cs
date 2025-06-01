using System;
using System.Collections.Generic;
using SpookyCore.EntitySystem;
using UnityEngine;

namespace SpookyCore.SystemLoader
{
    [Serializable]
    public struct PoolEntry
    {
        public EntityID ID;
        public int InitialSize;
    }
    
    [CreateAssetMenu(menuName = "SpookyCore/System Configs/Pool System/Pool Objects Config", fileName = "Pool_Objects_Config")]
    public class PoolObjectsConfig : ScriptableObject
    {
        [field: SerializeField] public List<PoolEntry> PoolEntries { get; private set; } = new();
    }
}