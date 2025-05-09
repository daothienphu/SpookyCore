using SpookyCore.SystemLoader;
using SpookyCore.UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace SpookyCore.EntitySystem
{
    public class EntityHealth : EntityComponentBase
    {
        #region Fields

        public Observable<float> HealthObservable;
        [SerializeField] private Image _healthBar;

        #endregion

        #region Life Cycle

        public override void OnStart()
        {
            base.OnStart();
            Debug.Log("subscribed");
            Entity.OnEntityStateChanged += OnEntityStateChanged;
            HealthObservable = new Observable<float>(Entity.Get<EntityData>().FinalStats.MaxHealth);
            UpdateHealthBar();
        }

        private void OnDisable()
        {
            if (Entity)
            {
                Debug.Log("unsubscribed");
                Entity.OnEntityStateChanged -= OnEntityStateChanged;
            }
        }

        #endregion

        #region Public Methods

        public virtual void TakeDamage(EntityID damageDealer, float dmg)
        {
            HealthObservable.Value = Mathf.Max(0, HealthObservable.Value - dmg);
            if (HealthObservable.Value == 0)
            {
                Entity.SetState(EntityBase.EntityState.Dead);
                
            }
            UpdateHealthBar();
        }

        #endregion

        #region Private Methods

        protected virtual void OnEntityStateChanged(EntityStateEvent ctx)
        {
            switch (ctx.NewState)
            {
                case EntityBase.EntityState.Spawned:
                    if (_healthBar)
                    {
                        _healthBar.transform.parent.gameObject.SetActive(true);
                    }
                    HealthObservable.Value = Entity.Get<EntityData>().FinalStats.MaxHealth;
                    UpdateHealthBar();
                    break;
                case EntityBase.EntityState.Dead:
                {
                    //_healthBar is only the health part, the parent will also contain the health bar background. 
                    if (_healthBar)
                    {
                        _healthBar.transform.parent.gameObject.SetActive(false);
                    }
                    
                    if (Entity.TryGet<EntityAnimationRunner>(out var animationRunner))
                    {
                        //animationRunner.TransitionTo(GameAnimation.IsDead);
                    }

                    if (Entity.TryGet<EntityItemDropper>(out var itemDropper))
                    {
                        itemDropper.DropItems();
                    }
                    
                    break;
                }
            }
        }

        private void UpdateHealthBar()
        {
            if (_healthBar)
            {
                _healthBar.fillAmount = HealthObservable.Value / Entity.Get<EntityData>().FinalStats.MaxHealth;
            }
        }

        #endregion
    }
}