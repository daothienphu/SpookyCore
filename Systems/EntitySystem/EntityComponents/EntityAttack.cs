using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityAttack : EntityComponent
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

        public void ExecuteAttack(Entity target = null)
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
    }
}