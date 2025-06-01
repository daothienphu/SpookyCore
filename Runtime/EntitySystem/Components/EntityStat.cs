using SpookyCore.EntitySystem.Utils.Stat;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityStat : EntityComponent
    {
        #region Fields

        [SerializeField] private EntityStatConfig _statConfig;        

        #endregion

        #region Life Cycle
        
        public override void OnAwake()
        {
            _statConfig = Instantiate(_statConfig);
        }

        #endregion

        #region Public Methods

        public T GetStats<T>() where T : EntityStatConfig => _statConfig as T;

        public bool TryGetStats<T>(out T stats) where T : EntityStatConfig
        {
            if (_statConfig is T t)
            {
                stats = t;
                return true;
            }
            stats = null;
            return false;
        }

        #endregion
        
    }
}