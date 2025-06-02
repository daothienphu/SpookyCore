using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    [CreateAssetMenu(menuName = "SpookyCore/System/Global Asset Registry/Scriptable Object Registry", fileName = "ScriptableObjectRegistry")]
    public class ScriptableObjectRegistry : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string Key;
            public ScriptableObject Asset;
        }

        public List<Entry> Objects;

        private Dictionary<string, ScriptableObject> _lookup;

        public T Get<T>(string key) where T : ScriptableObject
        {
            _lookup ??= Objects.ToDictionary(o => o.Key, o => o.Asset);
            return _lookup.TryGetValue(key, out var obj) ? obj as T : null;
        }
    }

}