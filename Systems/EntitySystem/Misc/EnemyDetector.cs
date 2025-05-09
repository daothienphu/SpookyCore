using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EnemyDetector : TriggerUtil
    {
        [SerializeField] private LayerMask DetectionLayer;
        [field: SerializeField] public List<EntityBase> FoundTargets;
        private Collider2D _collider2D;

        protected override void Start()
        {
            base.Start();
            _collider2D = GetComponent<Collider2D>();
            FoundTargets = new List<EntityBase>();
        }

        public void ToggleColliders(bool isEnabled)
        {
            _collider2D.enabled = isEnabled;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & DetectionLayer) == 0) return;
            if (!TryGetEntity(other.transform, out var entity)) return;
            
            if (FoundTargets.Any(t => entity == t)) return;
            FoundTargets.Add(entity);
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & DetectionLayer) == 0) return;
            if (!TryGetEntity(other.transform, out var entity)) return;
            
            if (FoundTargets.Contains(entity))
            {
                FoundTargets.Remove(entity);
            }
        }
    }
}