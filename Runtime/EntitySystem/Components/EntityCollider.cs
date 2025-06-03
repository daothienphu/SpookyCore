using System.Collections.Generic;
using SpookyCore.Runtime.Utilities;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityCollider : EntityComponent
    {
        #region Fields

        [SerializeField] internal ColliderListener _colliderListener;
        public Collider2D Collider2D;
        [field: SerializeField] public bool IsEnabled { get; private set; } = true;

        [SerializeField] internal bool _prepareRigidbody2D = true;
        
        public bool HasCollided => _collidedEntities.Count > 0;
        public Entity CollidedEntity => _collidedEntities.Count > 0 ? _collidedEntities[0] : null;
        public IReadOnlyList<Entity> CollidedEntities => _collidedEntities.AsReadOnly();
        
        private readonly List<Entity> _collidedEntities = new();

        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            ToggleCollider(true);
            Collider2D = _colliderListener.gameObject.GetComponent<Collider2D>();
        }

        #endregion

        #region Public Methods

        public virtual void ToggleCollider(bool isEnabled)
        {
            IsEnabled = isEnabled;
            _colliderListener.gameObject.SetActive(isEnabled);
        }
        
        public virtual void RegisterCollisionEnter(Collision2D collision)
        {
            if (collision.TryGetEntity(out var entity) && !_collidedEntities.Contains(entity))
            {
                _collidedEntities.Add(entity);
            }
        }

        public virtual void RegisterCollisionExit(Collision2D collision)
        {
            if (collision.TryGetEntity(out var entity))
            {
                _collidedEntities.Remove(entity);
            }
        }

        public virtual void ClearCollisions()
        {
            _collidedEntities.Clear();
        }

        #endregion
    }
}