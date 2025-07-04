using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [CreateAssetMenu(menuName = "SpookyCore/Entity System/Attack/Weapon (Melee) Config", fileName = "Melee_WeaponConfig")]
    public class MeleeWeaponConfig : WeaponConfig
    {
        public LayerMask DamageMask;
    }
}