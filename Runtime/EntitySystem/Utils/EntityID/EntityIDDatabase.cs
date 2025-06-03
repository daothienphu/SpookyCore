using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [CreateAssetMenu(menuName = "SpookyCore/Entity System/Entity ID Database", fileName = "EntityIDDatabase")]
    internal sealed class EntityIDDatabase : ScriptableObject
    {
        [Serializable]
        public class EntityIDCategory
        {
            public string CategoryName;
            public List<string> Entries = new();

            public EntityIDCategory(string name)
            {
                CategoryName = name;
            }
        }
        
        public List<EntityIDCategory> Categories = new();
    }
}