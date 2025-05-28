using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityCollider : EntityComponent
    {
        public bool HasCollided => _collidedEntities.Count > 0;
        public Entity CollidedEntity => _collidedEntities.Count > 0 ? _collidedEntities[0] : null;
        public IReadOnlyList<Entity> CollidedEntities => _collidedEntities.AsReadOnly();

        private readonly List<Entity> _collidedEntities = new();

        public virtual void RegisterCollisionEnter(Collision2D collision)
        {
            var otherEntity = collision.gameObject.GetComponentInParent<Entity>();
            if (otherEntity && !_collidedEntities.Contains(otherEntity))
            {
                _collidedEntities.Add(otherEntity);
            }
        }

        public virtual void RegisterCollisionExit(Collision2D collision)
        {
            var otherEntity = collision.gameObject.GetComponentInParent<Entity>();
            if (otherEntity)
            {
                _collidedEntities.Remove(otherEntity);
            }
        }

        public void ClearCollisions()
        {
            _collidedEntities.Clear();
        }
    }
}