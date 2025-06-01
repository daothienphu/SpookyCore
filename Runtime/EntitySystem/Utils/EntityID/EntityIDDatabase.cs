using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.EntitySystem.Utils.EntityID
{
    [CreateAssetMenu(fileName = "EntityIDDatabase", menuName = "SpookyCore/EntitySystem/Entity ID Database")]
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