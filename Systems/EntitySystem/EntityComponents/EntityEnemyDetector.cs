using SpookyCore.Utilities.Editor.Attributes;
using SpookyCore.Utilities.Editor.Gizmos;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityEnemyDetector : EntityComponentBase
    {
        #region Fields

        [SerializeField] protected EnemyDetector _enemyDetector;
        [SerializeField] protected float _viewDistance = 2;
        [SerializeField] protected float _fovAngle = 45;
        [SerializeField] protected float _refreshTargetThreshold = 0.25f;
        
        protected float _halfFOV;
        protected float _refreshTargetTimer;
        private EntityVisual _visual;
        private Collider2D _collider;

        #endregion

        #region Properties

        [ReadOnly] public bool HasDetectedEnemy;
        [ReadOnly] public EntityBase EnemyTarget;
        [ReadOnly] public float SqrDistanceToEnemy;

        #endregion

        #region Life Cycle

        public override void OnStart()
        {
            base.OnStart();
            _visual = Entity.Get<EntityVisual>();
            _halfFOV = _fovAngle / 2f;
            _collider = _enemyDetector.GetComponent<Collider2D>();
            if (_collider is CircleCollider2D circleCollider2D)
            {
                circleCollider2D.radius = Entity.Get<EntityData>().FinalStats.DetectionRange;
                _viewDistance = circleCollider2D.radius;
            }
        }

        public override void OnUpdate()
        {
            var culler = Entity.Get<EntityCuller>();
            if (culler && culler.HasJustChangedCullState)
            {
                _enemyDetector.ToggleColliders(!culler.ShouldBeCulled);
            }
            
            _refreshTargetTimer += Time.deltaTime;
            if (_refreshTargetTimer > _refreshTargetThreshold)
            {
                _refreshTargetTimer = 0;
                OnTargetTimerRefreshed();
            }
        }

        #endregion

        #region Private Methods

        protected virtual void OnTargetTimerRefreshed()
        {
            GetTarget();
        }
        
        private void GetTarget()
        {
            HasDetectedEnemy = true;
            EnemyTarget = null;
            if (SqrDistanceToEnemy == 0)
            {
                SqrDistanceToEnemy = float.MaxValue;
            }
            
            foreach (var t in _enemyDetector.FoundTargets)
            {
                if (IsWithinFieldOfView(t.transform))
                {
                    EnemyTarget = t;
                    SqrDistanceToEnemy = (t.transform.position - Entity.transform.position).sqrMagnitude;
                }
            }

            if (!EnemyTarget)
            {
                HasDetectedEnemy = false;
                SqrDistanceToEnemy = 0;
            }
        }

        private bool IsWithinFieldOfView(Transform target)
        {
            var dirToTarget = (target.position - Entity.transform.position).normalized;
            var angle = Vector3.Angle(dirToTarget, _visual.Heading);
            return angle <= _halfFOV;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!GlobalGizmoController.GetGizmoState("Entities/Nearby Enemy Detector")) return;
            
            if (!Entity) return;
            var originalColor = Gizmos.color;
            
            //Draw nearby targets
            if (EnemyTarget)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(Entity.transform.position, EnemyTarget.transform.position);
            }
            else
            {
                Gizmos.color = Color.blue;
                foreach (var t in _enemyDetector.FoundTargets)
                {
                    Gizmos.DrawLine(Entity.transform.position, t.transform.position);
                }
            }
            
            //Draw FOV
            Gizmos.color = Color.green;
            var leftBoundary = Quaternion.Euler(0, 0, -_halfFOV) * _visual.Heading * _viewDistance; 
            var rightBoundary = Quaternion.Euler(0, 0, _halfFOV) * _visual.Heading * _viewDistance; 
            Gizmos.DrawLine(Entity.transform.position, Entity.transform.position + leftBoundary); 
            Gizmos.DrawLine(Entity.transform.position, Entity.transform.position + rightBoundary);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Entity.transform.position, _viewDistance);
            
            Gizmos.color = originalColor;
        }
#endif
        #endregion
    }
}