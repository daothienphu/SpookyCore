using SpookyCore.Runtime.Utilities;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem.Attack
{
    public class Projectile : MonoBehaviour
    {
        private float _damage;
        private float _lifetime;
        private Vector2 _velocity;

        public void Initialize(Vector2 velocity, float damage, float lifetime)
        {
            _velocity = velocity;
            _damage = damage;
            _lifetime = lifetime;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.position += (Vector3)(_velocity * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetEntity(out var entity))
            {
                Debug.Log($"Projectile hit {entity.name} for {_damage} damage");
                Destroy(gameObject);
            }
        }
    }
}