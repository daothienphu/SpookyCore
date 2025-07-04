using SpookyCore.Runtime.Utilities;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class RangedWeaponBase : IRangedWeapon
    {
        private readonly RangedWeaponConfig _config;
        public int CurrentAmmo { get; private set; }
        private float _cooldownTimer;
        private float _reloadTimer;
        public bool IsReloading { get; private set; }
        private RaycastHit2D[] _results = new RaycastHit2D[5];
        private ContactFilter2D _filter;

        public RangedWeaponBase(RangedWeaponConfig config)
        {
            _config = config;
            CurrentAmmo = _config.MaxAmmo;
            _filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = _config.DamageMask,
            };
        }
        
        public virtual void Reload()
        {
            if (IsReloading || CurrentAmmo == _config.MaxAmmo) return;

            IsReloading = true;
            _reloadTimer = _config.ReloadTime;
        }

        public virtual void Attack(Vector3 position, Vector3 direction, Entity target = null)
        {
            if (_cooldownTimer > 0 || IsReloading || CurrentAmmo <= 0) return;

            _cooldownTimer = _config.Cooldown;
            CurrentAmmo--;

            if (_config.Type == RangedWeaponConfig.RangedType.Hitscan)
            {
                var hitSize = Physics2D.Raycast(position, direction, _filter, _results, _config.Range);
                if (hitSize > 0)
                {
                    for (var i = 0; i < hitSize; ++i)
                    {
                        var hit = _results[i];
                        if (hit.collider.TryGetEntity(out var entity))
                        {
                            Debug.Log($"damaging entity {entity.ID} for {_config.Damage} damage.");
                        }
                    }
                }
            }
            else if (_config.Type == RangedWeaponConfig.RangedType.Projectile)
            {
                //todo: spawn projectile here.
            }
        }

        public virtual void Tick(float deltaTime)
        {
            if (IsReloading)
            {
                _reloadTimer -= deltaTime;
                if (_reloadTimer <= 0)
                {
                    CurrentAmmo = _config.MaxAmmo;
                    IsReloading = false;
                }
            }
            
            _cooldownTimer -= deltaTime;
        }

        public bool IsOnCooldown()
        {
            return _cooldownTimer > 0;
        }
    }
}