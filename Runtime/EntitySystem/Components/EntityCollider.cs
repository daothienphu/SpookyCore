using System.Collections.Generic;
using SpookyCore.EntitySystem.Utils;
using SpookyCore.Utilities;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityCollider : EntityComponent
    {
        #region Fields

        [SerializeField] internal ColliderListener _colliderListener;

        [field: SerializeField] public bool IsEnabled { get; private set; } = true;
        public bool HasCollided => _collidedEntities.Count > 0;
        public Entity CollidedEntity => _collidedEntities.Count > 0 ? _collidedEntities[0] : null;
        public IReadOnlyList<Entity> CollidedEntities => _collidedEntities.AsReadOnly();
        
        private readonly List<Entity> _collidedEntities = new();

        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            ToggleCollider(true);
        }

        #endregion

        #region Public Methods

        public void ToggleCollider(bool isEnabled)
        {
            IsEnabled = isEnabled;
            _colliderListener.gameObject.SetActive(isEnabled);
        }

        public void RegisterTriggerEnter(Collider2D collider)
        {
            if (collider.TryGetEntity(out var entity) && !_collidedEntities.Contains(entity))
            {
                _collidedEntities.Add(entity);
            }
        }

        public void RegisterTriggerExit(Collider2D collider)
        {
            if (collider.TryGetEntity(out var entity))
            {
                _collidedEntities.Remove(entity);
            }
        }
        
        public void RegisterCollisionEnter(Collision2D collision)
        {
            if (collision.TryGetEntity(out var entity) && !_collidedEntities.Contains(entity))
            {
                _collidedEntities.Add(entity);
            }
        }

        public void RegisterCollisionExit(Collision2D collision)
        {
            if (collision.TryGetEntity(out var entity))
            {
                _collidedEntities.Remove(entity);
            }
        }

        public void ClearCollisions()
        {
            _collidedEntities.Clear();
        }

        #endregion
    }
}