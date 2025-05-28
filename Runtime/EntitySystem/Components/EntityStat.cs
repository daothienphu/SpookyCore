using SpookyCore.EntitySystem.Utils.Stat;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityStat : EntityComponent
    {
        [SerializeField] private EntityStatConfig _statConfig;
        public EntityStatConfig Stats;

        public T GetData<T>() where T : EntityStatConfig => Stats as T;

        public override void OnAwake()
        {
            Stats = Instantiate(_statConfig);
        }
    }
}