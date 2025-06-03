using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public enum AttackType { Melee, Ranged }

    [CreateAssetMenu(menuName = "SpookyCore/Entity System/Attack/Attack Config", fileName = "Attack Config")]
    public class EntityAttackConfig : ScriptableObject
    {
        public AttackType Type;
        public float Cooldown = 0.5f;
        public float Damage = 10f;

        [Header("Melee Settings")]
        public MeleeAttackSequence MeleeSequence;

        [Header("Ranged Settings")]
        public GameObject ProjectilePrefab;
        public float ProjectileSpeed = 10f;
        public float ProjectileLifetime = 2f;
    }

    [System.Serializable]
    public class MeleeAttackSequence
    {
        public MeleeAttackFrame[] Frames;
    }

    [System.Serializable]
    public class MeleeAttackFrame
    {
        public float Time;
        public Vector2 Offset;
        public Vector2 Size;
        public float Duration;
        public Vector2 Knockback;
    }
}