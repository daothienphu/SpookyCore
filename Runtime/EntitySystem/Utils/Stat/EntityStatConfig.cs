using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpookyCore.EntitySystem.Utils.Stat
{
    [CreateAssetMenu(menuName = "SpookyCore/Components/Stat/Entity Stat Config", fileName = "EntityStat_Config")]
    public class EntityStatConfig : ScriptableObject
    {
        [SerializeField] private List<Stat> _stats;
        private Dictionary<Type, Stat> _statCache;

        public T GetStat<T>() where T : Stat
        {
            _statCache ??= _stats.ToDictionary(stat => stat.GetType(), stat => stat);

            return _statCache.TryGetValue(typeof(T), out var stat) ? (T)stat : null;
        }

        public bool TryGetStat<T>(out T stat) where T : Stat
        {
            stat = GetStat<T>();
            return stat != null;
        }

        public List<Stat> GetAllStats() => _stats;
    }
}