using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public abstract class AttackConfig : ScriptableObject
    {
        public float Cooldown;
        public abstract Attack CreateAttack(EntityAttack entityAttack);
    }
}