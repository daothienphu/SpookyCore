using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public abstract class WeaponConfig : ScriptableObject
    {
        public float Damage = 10f;
        public float Range = 5f;
        public float Cooldown = 1f;
    }
}