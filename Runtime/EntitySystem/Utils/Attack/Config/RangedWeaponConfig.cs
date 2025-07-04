using System;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [CreateAssetMenu(menuName = "SpookyCore/Entity System/Attack/Weapon (Ranged) Config", fileName = "Ranged_WeaponConfig")]
    public class RangedWeaponConfig : WeaponConfig
    {
        [Serializable]
        public enum RangedType { Hitscan, Projectile }

        [Header("General Config")]
        public RangedType Type;
        public int MaxAmmo;
        public float ReloadTime;
        public LayerMask DamageMask;
        
        [Header("Projectile type config")]
        public EntityID ProjectileID;
        public float ProjectileSpeed = 2f;
    }
}