using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public abstract class WeaponFactory : ScriptableObject
    {
        public WeaponConfig Config;
        protected IWeapon _weapon;
        public abstract IWeapon GetWeapon();
    }
}