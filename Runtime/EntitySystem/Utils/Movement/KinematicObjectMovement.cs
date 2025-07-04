using SpookyCore.Runtime.Utilities;
using System;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class KinematicObjectMovement : MonoBehaviour
    {
        #region Structs

        protected struct RaycastOrigins 
        {
            public Vector2 TopLeft, TopRight;
            public Vector2 BottomLeft, BottomRight;
        }

        public struct CollisionInfo 
        {
            public bool Above, Below, Left, Right;
            public bool StartGroundedThisFrame;
            public bool WasGroundedLastFrame;

            public void Reset() 
            {
                Above = Below = Left = Right = StartGroundedThisFrame = false;
            }
        }

        #endregion

        #region Fields

        [Header("Kinematic Object Settings")]
        [SerializeField] protected bool _drawDebug = true;
        [SerializeField] protected LayerMask _collisionMask;
        [SerializeField] protected Collider2D _collider;
        [SerializeField] protected float _skinWidth = 0.02f;
        [SerializeField] protected int _horizontalRayCount = 4;
        [SerializeField] protected int _verticalRayCount = 4;
        [ReadOnly, SerializeField] protected float _horizontalRaySpacing;
        [ReadOnly, SerializeField] protected float _verticalRaySpacing;
        [ReadOnly, SerializeField] protected float _deltaTime = 0.1f;
        
        protected RaycastOrigins _raycastOrigins;
        protected const float FloatFudgeFactor = 0.001f;

        private bool _initialized;
 
        private RaycastHit2D[] _hits = new RaycastHit2D[10];
        private ContactFilter2D _contactFilter;
        
        #endregion

        #region Properties

        public CollisionInfo Collisions;

        #endregion

        #region Life Cycle

        private void Start()
        {
            CalculateRaySpacing();
            _contactFilter = new ContactFilter2D()
            {
                useLayerMask = true,
                layerMask = _collisionMask
            };
        }

        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Moves the current object by the deltaMovement, stops if collided with other objects.
        /// </summary>
        /// <param name="deltaMovement"></param>
        /// <param name="onCollideLeft"></param>
        /// <param name="onCollideRight"></param>
        /// <param name="onCollideAbove"></param>
        /// <param name="onCollideBelow"></param>
        /// <returns> The adjusted and collision-aware deltaMovement.</returns>
        public Vector2 Move(Vector2 deltaMovement, Action onCollideLeft = null, Action onCollideRight = null, Action onCollideAbove = null, Action onCollideBelow = null)
        {
            Collisions.WasGroundedLastFrame = Collisions.Below;
            Collisions.Reset();
            UpdateRaycastOrigins();
            
            if (deltaMovement.x != 0f) 
            {
                HorizontalCollisions(ref deltaMovement);
            }
            VerticalCollisions(ref deltaMovement);

            if (Collisions.Above) onCollideAbove?.Invoke();
            if (Collisions.Below) onCollideBelow?.Invoke();
            if (Collisions.Left) onCollideLeft?.Invoke();
            if (Collisions.Right) onCollideRight?.Invoke();
            
            transform.Translate(deltaMovement);

            if (Collisions is { WasGroundedLastFrame: false, Below: true })
            {
                Collisions.StartGroundedThisFrame = true;
            }

            return deltaMovement;
        }

        #endregion

        #region Protected Methods

        protected void CalculateRaySpacing() {
            var bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);

            _horizontalRayCount =    Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
            _verticalRayCount =      Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);

            _horizontalRaySpacing =  bounds.size.y / (_horizontalRayCount - 1);
            _verticalRaySpacing =    bounds.size.x / (_verticalRayCount - 1);
        }
        
        protected void HorizontalCollisions(ref Vector2 velocity)
        {
            var direction = Mathf.Sign(velocity.x);
            var rayDistance = Mathf.Abs(velocity.x) + _skinWidth;
            var initialRayOrigin = direction < 0 
                ? _raycastOrigins.BottomLeft 
                : _raycastOrigins.BottomRight;

            for (var i = 0; i < _horizontalRayCount; ++i)
            {
                var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * _horizontalRaySpacing);

                if (_drawDebug)
                {
                    Debug.DrawRay(ray, Vector2.right * (direction * rayDistance), Color.red);
                }
                
                var size = Physics2D.Raycast(ray, Vector2.right * direction, _contactFilter, _hits, rayDistance);

                for (var j = 0; j < size; ++j)
                {
                    var hit = _hits[j];
                    if (hit.rigidbody &&
                        hit.rigidbody.gameObject == gameObject) continue;

                    if (velocity.x > rayDistance) continue;
                    
                    velocity.x = hit.distance * direction;
                    rayDistance = Mathf.Abs(velocity.x);
                    
                    if (direction < 0)
                    {
                        velocity.x += _skinWidth;
                        Collisions.Left = true;
                    }
                    else if (direction > 0)
                    {
                        velocity.x -= _skinWidth;
                        Collisions.Right = true;
                    }
                    
                    if (rayDistance < _skinWidth + FloatFudgeFactor)
                    {
                        velocity.x = 0;
                        return;
                    }
                }
            }
        }

        protected void VerticalCollisions(ref Vector2 velocity)
        {
            var direction = velocity.y > 0.0f ? 1 : -1;
            var rayDistance = Mathf.Abs(velocity.y) + _skinWidth;
            var initialRayOrigin = direction > 0
                ? _raycastOrigins.TopLeft
                : _raycastOrigins.BottomLeft;

            initialRayOrigin.x += velocity.x;

            for (var i = 0; i < _verticalRayCount; ++i)
            {
                var ray = new Vector2(initialRayOrigin.x + i * _verticalRaySpacing, initialRayOrigin.y);

                if (_drawDebug)
                {
                    Debug.DrawRay(ray, Vector2.up * (direction * rayDistance), Color.red);
                }

                var size = Physics2D.Raycast(ray, Vector2.up * direction, _contactFilter, _hits, rayDistance);

                for (var j = 0; j < size; ++j)
                {
                    var hit = _hits[j];
                    if (hit.rigidbody && 
                        hit.rigidbody.gameObject == gameObject) continue;

                    if (hit.distance > rayDistance) continue;

                    velocity.y = hit.distance * direction;
                    rayDistance = Mathf.Abs(velocity.y);
                    
                    if (direction > 0)
                    {
                        velocity.y -= _skinWidth;
                        Collisions.Above = true;
                    }
                    else
                    {
                        velocity.y += _skinWidth;
                        Collisions.Below = true;
                    }

                    // Velocity = velocity / _deltaTime;
                    
                    if (rayDistance < _skinWidth + FloatFudgeFactor)
                    {
                        velocity.y = 0;
                        return;
                    }
                }
            }
        }
        
        protected void UpdateRaycastOrigins()
        {
            var bounds = _collider.bounds;
            bounds.Expand(_skinWidth * -2);

            _raycastOrigins.BottomLeft =   new Vector2 (bounds.min.x, bounds.min.y);
            _raycastOrigins.BottomRight =  new Vector2 (bounds.max.x, bounds.min.y);
            _raycastOrigins.TopLeft =      new Vector2 (bounds.min.x, bounds.max.y);
            _raycastOrigins.TopRight =     new Vector2 (bounds.max.x, bounds.max.y);
        }

        #endregion
    }
}