using System;
using SpookyCore.Runtime.Systems;
using SpookyCore.Runtime.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SpookyCore.Runtime.EntitySystem
{
    [RequireComponent(typeof(EntityStat))]
    public class EntityHealth : EntityComponent
    {
        #region Fields

        public float Health;
        public Slider HealthBar;
        [SerializeField] protected bool _setAnimation;

        private EntityStat _stat;
        private EntityAnimation _animation;

        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            _stat = Entity.Get<EntityStat>();
            _animation = Entity.Get<EntityAnimation>();
            
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
            if (Entity.State != Entity.EntityState.Alive) return;
            
            Health = Mathf.Max(0, Health - dmg);
            SetAnimation("Damaged");
            UpdateHealthBar();
            
            if (Health <= 0)
            {
                Entity.SetState(Entity.EntityState.Dead);
                // if (_animation && _setAnimation)
                // {
                //     SetAnimation("Dead", () => Entity.SetState(Entity.EntityState.Dead));
                // }
                // else
                // {
                //     Entity.SetState(Entity.EntityState.Dead);
                // }
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

        protected virtual void SetAnimation(string state, Action onAfterPlay = null)
        {
            if (_setAnimation && _animation)
            {
                if (!_animation.IsAnimationPlaying(state))
                {
                    _animation.PlayAnimation(state, onAfterPlay: onAfterPlay);
                }
            }
        }

        #endregion
    }
}