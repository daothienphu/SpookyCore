using SpookyCore.EntitySystem.Utils.Stat;
using UnityEngine;
using UnityEngine.UI;

namespace SpookyCore.EntitySystem
{
    [RequireComponent(typeof(EntityStat))]
    public class EntityHealth : EntityComponent
    {
        #region Fields

        public float Health;
        public Slider HealthBar;

        private EntityStat _stat;

        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            _stat = Entity.Get<EntityStat>();
            
            Entity.OnEntityStateChanged += OnEntityStateChanged;

            Health = _stat.GetStats<EntityStatConfig>().Health.Current;
            UpdateHealthBar();
        }

        private void OnDisable()
        {
            if (Entity)
            {
                Entity.OnEntityStateChanged -= OnEntityStateChanged;
            }
        }

        #endregion

        #region Public Methods

        public virtual void TakeDamage(EntityID damageSource, float dmg)
        {
            Health = Mathf.Max(0, Health - dmg);
            UpdateHealthBar();
            
            if (Health <= 0)
            {
                Entity.SetState(Entity.EntityState.Dead);
            }
        }

        #endregion

        #region Private Methods

        protected virtual void OnEntityStateChanged(EntityStateEvent ctx)
        {
            switch (ctx.NewState)
            {
                case Entity.EntityState.Spawned:
                {
                    HealthBar?.gameObject.SetActive(true);

                    Health = Entity.Get<EntityStat>().GetStats<EntityStatConfig>().Health.Current;
                    UpdateHealthBar();
                    break;
                }
                case Entity.EntityState.Dead:
                {
                    HealthBar?.gameObject.SetActive(false);
                    break;
                }
            }
        }

        protected virtual void UpdateHealthBar()
        {
            if (!HealthBar) return;
            HealthBar.value = Health / _stat.GetStats<EntityStatConfig>().Health.Current;
        }

        #endregion
    }
}