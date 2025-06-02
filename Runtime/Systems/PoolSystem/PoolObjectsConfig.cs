using System;
using System.Collections.Generic;
using SpookyCore.Runtime.EntitySystem;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
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