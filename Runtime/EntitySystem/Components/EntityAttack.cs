using System.Collections;
using SpookyCore.Runtime.Utilities;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem.Attack
{
    [RequireComponent(typeof(Entity))]
    public class EntityAttack : EntityComponent
    {
        public EntityAttackConfig AttackConfig;
        private float _lastAttackTime;
        private Coroutine _attackCoroutine;

        public void AttackDirection(Vector2 direction)
        {
            if (CanAttack())
            {
                _attackCoroutine = StartCoroutine(ExecuteAttack(direction.normalized));
            }
        }

        public void AttackTarget(Entity target)
        {
            if (target == null) return;
            var dir = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            AttackDirection(dir);
        }

        private bool CanAttack()
        {
            return Time.time >= _lastAttackTime + AttackConfig.Cooldown;
        }

        private IEnumerator ExecuteAttack(Vector2 direction)
        {
            _lastAttackTime = Time.time;

            switch (AttackConfig.Type)
            {
                case AttackType.Melee:
                    yield return StartCoroutine(ExecuteMeleeAttack(direction));
                    break;
                case AttackType.Ranged:
                    ExecuteRangedAttack(direction);
                    break;
            }
        }

        private IEnumerator ExecuteMeleeAttack(Vector2 direction)
        {
            foreach (var frame in AttackConfig.MeleeSequence.Frames)
            {
                yield return new WaitForSeconds(frame.Time);

                var hitboxCenter = (Vector2)transform.position + Vector2.Scale(frame.Offset, direction);
                var hits = Physics2D.OverlapBoxAll(hitboxCenter, frame.Size, 0f);
                foreach (var hit in hits)
                {
                    if (hit.TryGetEntity(out var entity) && entity != Entity)
                    {
                        // TODO: Apply damage, knockback, effects
                        Debug.Log($"Hit {entity.name} with {AttackConfig.Damage} damage");
                    }
                }

                yield return new WaitForSeconds(frame.Duration);
            }
        }

        private void ExecuteRangedAttack(Vector2 direction)
        {
            var projectileGO = Instantiate(AttackConfig.ProjectilePrefab, transform.position, Quaternion.identity);
            if (projectileGO.TryGetComponent(out Projectile projectile))
            {
                projectile.Initialize(direction * AttackConfig.ProjectileSpeed, AttackConfig.Damage, AttackConfig.ProjectileLifetime);
            }
        }
    }
}