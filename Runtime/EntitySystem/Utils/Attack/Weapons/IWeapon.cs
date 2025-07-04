using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public interface IWeapon
    {
        void Attack(Vector3 position, Vector3 direction, Entity target = null);
        
        /// <summary>
        /// For cooldown and reload
        /// </summary>
        /// <param name="deltaTime"></param>
        void Tick(float deltaTime);

        bool IsOnCooldown();
    }
    
    public interface IMeleeWeapon : IWeapon
    {
        
    }

    public interface IRangedWeapon : IWeapon
    {
        int CurrentAmmo { get; }
        bool IsReloading { get; }
        void Reload();
    }
}