using SpookyCore.Runtime.Utilities;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class MeleeWeaponBase : IMeleeWeapon
    {
        private readonly MeleeWeaponConfig _config;
        private float _cooldownTimer;
        private RaycastHit2D[] _results = new RaycastHit2D[5];
        private ContactFilter2D _filter;
        
        public MeleeWeaponBase(MeleeWeaponConfig config)
        {
            _config = config;
            _filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = _config.DamageMask
            };
        }
        
        public virtual void Attack(Vector3 position, Vector3 direction, Entity target = null)
        {
            if (_cooldownTimer > 0) return;
            _cooldownTimer = _config.Cooldown;
            
            var hitSize = Physics2D.Raycast(position, direction, _filter, _results, _config.Range);
            if (hitSize > 0)
            {
                for (var i = 0; i < hitSize; ++i)
                {
                    var hit = _results[i];
                    if (hit.collider.TryGetEntity(out var entity))
                    {
                        if (entity.TryGetComponent(out EntityHealth health))
                        {
                            health.TakeDamage(EntityID.Player, _config.Damage);
                        }
                    }
                }
            }
        }

        public virtual void Tick(float deltaTime)
        {
            _cooldownTimer -= deltaTime;
        }

        public bool IsOnCooldown()
        {
            return _cooldownTimer > 0;
        }
    }
}