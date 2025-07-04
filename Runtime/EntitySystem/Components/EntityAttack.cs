using SpookyCore.Runtime.Utilities;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [RequireComponent(typeof(Entity))]
    public class EntityAttack : EntityComponent
    {
        [SerializeField] protected WeaponFactory _weaponFactory;
        [SerializeField] protected bool _setAnimation;
        protected IWeapon _weapon;
        protected EntityInputReceiver _input;
        protected EntityAnimation _animation;

        public override void OnAwake()
        {
            _weapon = _weaponFactory.GetWeapon();
            _input = Entity.Get<EntityInputReceiver>();
            _animation = Entity.Get<EntityAnimation>();
        }

        public override void OnUpdate()
        {
            _weapon.Tick(Time.deltaTime);

            if (_input && _input.AttackPressed)
            {
                var position = _input.GetMousePosition();
                var dir = position - transform.position.V2();
                AttackDirection(dir);
            }
        }

        public void AttackDirection(Vector2 dir)
        {
            if (_weapon.IsOnCooldown()) return;
            
            _weapon.Attack(transform.position, dir);
            
            if (_setAnimation && _animation)
            {
                var state = "Attack";
                if (!_animation.IsAnimationPlaying(state))
                {
                    _animation.PlayAnimation(state);
                }
            }
        }

        public void AttackTarget(Entity target)
        {
            if (_weapon.IsOnCooldown()) return;

            var dir = target.transform.position - transform.position;
            
            _weapon.Attack(transform.position, dir);
            
            if (_setAnimation && _animation)
            {
                var state = "Attack";
                if (!_animation.IsAnimationPlaying(state))
                {
                    _animation.PlayAnimation(state);
                }
            }
        }

        public void Reload()
        {
            if (_weapon is IRangedWeapon rangedWeapon)
            {
                rangedWeapon.Reload();
            }
        }

        public void SetWeaponFactory(WeaponFactory factory)
        {
            _weaponFactory = factory;
            _weapon = _weaponFactory.GetWeapon();
        }
    }
}