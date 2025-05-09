using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityAttack : EntityComponentBase
    {
        #region Fields

        [SerializeField] private AttackConfig _attackConfig;
        private Attack _currentAttack;
        private float _cooldownTimer;

        #endregion

        #region Life Cycle

        public override void OnUpdate()
        {
            // if (_cooldownTimer > 0)
            // {
            //     Debug.Log("attack cooldown");
            //     _cooldownTimer -= Time.deltaTime;
            //     return;
            // }
            
            if (_currentAttack == null) return;

            if (_currentAttack.State is Attack.AttackState.Finished or Attack.AttackState.Stopped)
            {
                _cooldownTimer = _attackConfig.Cooldown;
                return;
            }
            
            _currentAttack.Update();
        }

        #endregion

        #region Public Methods

        public void ExecuteAttack(EntityBase target = null)
        {
            if (_currentAttack is { State: Attack.AttackState.Running })
            {
                return;
            }

            _currentAttack ??= _attackConfig.CreateAttack(this);
            
            _currentAttack.Start(target);
        }

        public void StopCurrentAttack()
        {
            _currentAttack?.Stop();
        }

        #endregion
        

        
        
        
        
        
        // #region Fields
        //
        // [SerializeField] protected AttackConfig _attackConfig;
        // protected EntityData _data;
        //
        // #endregion
        //
        // #region Properties
        //
        // public bool IsAttacking { get; protected set; }
        //
        // #endregion
        //
        // #region Life Cycle
        //
        // public override void OnStart()
        // {
        //     _data = Entity.Get<EntityData>();
        //     if (!_attackConfig)
        //     {
        //         Debug.Log($"Attack config not assigned on {Entity.gameObject.name}.");
        //     }
        //     else
        //     {
        //         _attackConfig = _attackConfig.Clone();
        //         //_attackConfig.Init();
        //     }
        // }
        //
        // #endregion
        //
        // #region Public Methods
        //
        // public virtual void StartAttack(Vector3 targetPosition, Action onFinishedCallback = null)
        // {
        //     if (!CheckBeforeAttack()) return;
        //
        //     if (IsInAttackRange(targetPosition))
        //     {
        //         IsAttacking = true;
        //         //_attackConfig.ExecuteAttack(this, targetPosition);
        //     }
        //
        //     IsAttacking = false;
        //     onFinishedCallback?.Invoke();
        // }
        //
        // public virtual void StartAttack(EntityBase target, Action onFinishedCallback = null) => StartAttack(target.transform.position, onFinishedCallback);
        // // {
        // //     if (!CheckBeforeAttack()) return;
        // //     
        // //     if (target && IsInAttackRange(target))
        // //     {
        // //         //_target = target;
        // //         IsAttacking = true;
        // //         _attackConfig.ExecuteAttack(this, target);
        // //     }
        // // }
        //
        // public virtual bool IsInAttackRange(EntityBase target)
        // {
        //     return target && IsInAttackRange(target.transform.position);
        // }
        //
        // public virtual bool IsInAttackRange(Vector3 targetPosition)
        // {
        //     return (targetPosition - Entity.transform.position).sqrMagnitude <= _data.FinalStats.AttackRange * _data.FinalStats.AttackRange;
        // }
        //
        // #endregion
        //
        // #region Private Methods
        //
        // private bool CheckBeforeAttack()
        // {
        //     if (IsAttacking) return false;
        //     
        //     if (!_attackConfig)
        //     {
        //         Debug.Log($"Attack config not assigned on {Entity.gameObject.name}.");
        //         return false;
        //     }
        //
        //     return true;
        // }
        //
        // #endregion
    }
}