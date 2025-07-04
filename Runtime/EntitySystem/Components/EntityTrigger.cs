using System.Collections.Generic;
using SpookyCore.Runtime.Utilities;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityTrigger : EntityComponent
    {
        #region Fields

        [SerializeField] internal ColliderListener _colliderListener;
        public Collider2D Collider2D;
        [field: SerializeField] public bool IsEnabled { get; private set; } = true;

        [SerializeField] internal bool _prepareRigidbody2D = true;
        [SerializeField] private LayerMask _layerMask;
        
        protected readonly List<Entity> _detectedEntities = new();
        
        #endregion

        #region Properties

        public bool HasTriggered => _detectedEntities.Count > 0;
        public Entity FirstDetectedEntity => HasTriggered ? _detectedEntities[0] : null;
        public Entity ClosestDetectedEnemy => GetClosestEntity();
        public IReadOnlyList<Entity> DetectedEnemies => _detectedEntities.AsReadOnly();

        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            ToggleDetector(true);
            Collider2D = _colliderListener.gameObject.GetComponent<Collider2D>();
        }

        #endregion

        #region Public Methods

        public virtual void ToggleDetector(bool isEnabled)
        {
            IsEnabled = isEnabled;
            _colliderListener.gameObject.SetActive(isEnabled);
        }
        
        public virtual void RegisterTriggerEnter(Collider2D other)
        {
            if (other.TryGetEntity(out var entity) &&
                entity != Entity &&
                !_detectedEntities.Contains(entity))
            {
                _detectedEntities.Add(entity);
                OnEntityEntered(entity);
            }
        }

        public virtual void RegisterTriggerExit(Collider2D other)
        {
            if (other.TryGetEntity(out var entity))
            {
                _detectedEntities.Remove(entity);
                OnEntityExited(entity);
            }
        }

        public virtual void ClearDetections()
        {
            _detectedEntities.Clear();
        }

        #endregion

        #region Private Methods

        protected virtual void OnEntityEntered(Entity entity)
        {
            
        }

        protected virtual void OnEntityExited(Entity entity)
        {
            
        }
        
        protected virtual Entity GetClosestEntity()
        {
            if (_detectedEntities == null || _detectedEntities.Count == 0)
            {
                return null;
            }
            
            var minDistance = float.MaxValue;
            Entity closestEnemy = null;
            foreach (var entity in _detectedEntities)
            {
                var distance = (entity.transform.position - transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = entity;
                }
            }

            return closestEnemy;
        }

        #endregion
    }
}