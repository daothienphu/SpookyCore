using System.Collections.Generic;
using UnityEngine;
using SpookyCore.Runtime.Utilities;

namespace SpookyCore.Runtime.EntitySystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityEnemyDetector : EntityComponent
    {
        #region Fields

        [SerializeField] internal ColliderListener _colliderListener;
        public Collider2D Collider2D;
        [field: SerializeField] public bool IsEnabled { get; private set; } = true;

        private readonly List<Entity> _detectedEnemies = new();

        protected EntityStat _stat;
        
        #endregion

        #region Properties

        public bool HasDetectedEnemies => _detectedEnemies.Count > 0;
        public Entity FirstDetectedEnemy => HasDetectedEnemies ? _detectedEnemies[0] : null;
        public Entity ClosestDetectedEnemy => GetClosetEnemy();
        public IReadOnlyList<Entity> DetectedEnemies => _detectedEnemies.AsReadOnly();

        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            _stat = Entity.Get<EntityStat>();
            ToggleDetector(true);
            Collider2D = _colliderListener.GetComponent<Collider2D>();
        }

        public override void OnStart()
        {
            ((CircleCollider2D)Collider2D).radius = _stat?.GetStats<EntityStatConfig>().VisionRange.Current ?? 1;
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
                entity.ID.IsEnemy() &&
                !_detectedEnemies.Contains(entity))
            {
                _detectedEnemies.Add(entity);
            }
        }

        public virtual void RegisterTriggerExit(Collider2D other)
        {
            if (other.TryGetEntity(out var entity))
            {
                _detectedEnemies.Remove(entity);
            }
        }

        public virtual void ClearDetections()
        {
            _detectedEnemies.Clear();
        }

        #endregion

        #region Private Methods

        protected virtual Entity GetClosetEnemy()
        {
            if (_detectedEnemies == null || _detectedEnemies.Count == 0)
            {
                return null;
            }
            
            var minDistance = float.MaxValue;
            Entity closestEnemy = null;
            foreach (var entity in _detectedEnemies)
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