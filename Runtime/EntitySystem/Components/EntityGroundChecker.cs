using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityGroundChecker : EntityComponent
    {
        [SerializeField] private Transform _checkPoint;
        [SerializeField] private float _checkRadius = 0.2f;
        [SerializeField] private LayerMask _groundMask;

        [field: SerializeField] public bool IsGrounded { get; private set; }

        public void UpdateCheck()
        {
            IsGrounded = Physics2D.OverlapCircle(_checkPoint.position, _checkRadius, _groundMask);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (!_checkPoint) return;

            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_checkPoint.position, _checkRadius);
        }
#endif
    }
}