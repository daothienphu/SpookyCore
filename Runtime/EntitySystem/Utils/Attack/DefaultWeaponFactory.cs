using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [CreateAssetMenu(menuName = "SpookyCore/Entity System/Attack/Default Weapon Factory", fileName = "Default Weapon Factory")]
    public class DefaultWeaponFactory : WeaponFactory
    {
        public override IWeapon GetWeapon()
        {
            if (_weapon != null) return _weapon;
            
            switch (Config)
            {
                case MeleeWeaponConfig config:
                    _weapon = new MeleeWeaponBase(config);
                    break;
                case RangedWeaponConfig config:
                    _weapon = new RangedWeaponBase(config);
                    break;
            }

            return _weapon;
        }

        public void ResetWeaponInstance()
        {
            _weapon = null;
        }
    }
}