using SpookyCore.Utilities.Editor.Attributes;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class SimpleCharacterController2D : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _drawDebug = true;
        
        [Header("General Settings")]
        public Collider2D Collider2D;
        public float SkinWidth = 0.05f;
        
        [Header("Horizontal Movement Settings")]
        public float WalkSpeed = 4f;
        public float RunSpeed = 6f;
        public int WallRayCount = 3;
        
        [Header("Ground Check")]
        public LayerMask GroundMask;
        public int GroundRayCount = 2;
        
        [Header("Jump Settings")]
        public float DesiredJumpHeight = 2f;
        public float TimeToApex = 0.3f;
        public int MaxJumps = 2;
        [SerializeField] private float MaxFallSpeed = -20f;
        [SerializeField] private float CoyoteTime = 0.1f;
        [SerializeField] private float JumpBufferTime = 0.1f;
        
        [Header("Dash Settings")]
        public float DashForce = 20f;
        public float DashDuration = 0.2f;
        public float DashCooldown = 0.5f;
        
        [Header("Inputs")]
        [ReadOnly, SerializeField] private float _moveHorizontalInput;
        [ReadOnly, SerializeField] private bool _runHeldInput;
        [ReadOnly, SerializeField] private bool _jumpHeldInput;
        [ReadOnly, SerializeField] private float _lastJumpRequestedTime = -999f;
        [ReadOnly, SerializeField] private bool _dashRequested;
        [ReadOnly, SerializeField] private Vector2 _dashDirection;

        [Header("Internal States")]
        [ReadOnly, SerializeField] private Vector2 _velocity;
        [ReadOnly, SerializeField] private bool _grounded;
        [ReadOnly, SerializeField] private float _calculatedGravity;
        [ReadOnly, SerializeField] private float _calculatedJumpForce;
        [ReadOnly, SerializeField] private int _jumpCount;
        [ReadOnly, SerializeField] private float _lastGroundedTime;
        [ReadOnly, SerializeField] private bool _dashing;
        [ReadOnly, SerializeField] private float _dashEndTime = -999f;
        [ReadOnly, SerializeField] private float _lastDashTime = -999f;
        [ReadOnly, SerializeField] private float _deltaTime;
        
        private const float CollisionFudge = 0.001f;
        
        #endregion

        #region Properties
        
        public Vector2 Velocity => _velocity;
        public bool IsGrounded => _grounded;
        public bool IsDashing => _dashing;
        public bool IsJumping => !_grounded && _velocity.y > 0f;
        public bool IsFalling => !_grounded && _velocity.y < 0f;

        #endregion

        #region Life Cycle

        private void Awake()
        {
            CalculatePhysicsConstants();
        }
        
#if UNITY_EDITOR
        void OnValidate()
        {
            CalculatePhysicsConstants();
        }
#endif
        
        private void CalculatePhysicsConstants()
        {
            _calculatedGravity = -(2 * DesiredJumpHeight) / (TimeToApex * TimeToApex);
            _calculatedJumpForce = Mathf.Abs(_calculatedGravity) * TimeToApex;
        }
        
        public void Tick(float dt)
        {
            _deltaTime = dt;
            CheckGround();
            
            HandleDash();
            if (!_dashing)
            {
                ApplyHorizontalMovementWithPrediction();
                ApplyGravity();
                HandleJump();
                CheckCeiling();
            }
            
            _dashRequested = false;
        }

        #endregion
        
        #region Public Methods

        public void SetInputs(float horizontal, bool run)
        {
            _moveHorizontalInput = horizontal;
            _runHeldInput = run;
        }

        public void RequestJump(bool held)
        {
            if (held && !_jumpHeldInput)
            {
                _lastJumpRequestedTime = Time.time;
            }
            _jumpHeldInput = held;
        }
        
        public void RequestDash(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
            {
                direction = Vector2.right;
            }
            _dashRequested = true;
            _dashDirection = direction.normalized;
        }

        #endregion

        #region Private Methods
        
        private void CheckCeiling()
        {
            if (_velocity.y <= 0f) return;

            var bounds = Collider2D.bounds;
            var rayLength = _velocity.y * _deltaTime + SkinWidth;
            var spacing = (bounds.size.x - 2f * SkinWidth) / (GroundRayCount - 1);
            var originLeft = new Vector2(bounds.min.x + SkinWidth, bounds.max.y - rayLength);

            for (var i = 0; i < GroundRayCount; i++)
            {
                var origin = new Vector2(originLeft.x + i * spacing, originLeft.y);
                if (_drawDebug)
                    Debug.DrawRay(origin, Vector2.up * rayLength, Color.cyan);

                var hit = Physics2D.Raycast(origin, Vector2.up, rayLength, GroundMask);

                if (hit)
                {
                    var penetration = rayLength - hit.distance;
                    if (penetration > 0f)
                    {
                        transform.position -= Vector3.up * penetration;
                        _velocity.y = 0f;
                    }
                    break;
                }
            }
        }
        
        private void CheckGround()
        {
            var bounds = Collider2D.bounds;
            var rayLength = Mathf.Abs(_velocity.y * _deltaTime) + SkinWidth;
            var spacing = (bounds.size.x - 2 * SkinWidth) / (GroundRayCount - 1);
            var originLeft = new Vector2(bounds.min.x + SkinWidth, bounds.min.y + rayLength);

            var wasGrounded = _grounded;
            _grounded = false;

            for (var i = 0; i < GroundRayCount; i++)
            {
                var origin = new Vector2(originLeft.x + spacing * i, originLeft.y);
                if (_drawDebug)
                {
                    Debug.DrawRay(origin, Vector2.down * rayLength, Color.red);
                }

                var hit = Physics2D.Raycast(origin, Vector2.down, rayLength, GroundMask);

                if (!hit) continue;
                _grounded = true;

                var penetration = rayLength - hit.distance;
                if (penetration > 0f)
                {
                    transform.position += Vector3.up * penetration;
                    _velocity.y = 0f;
                }

                break; 
            }

            if (_grounded)
            {
                _lastGroundedTime = Time.time;
                if (!wasGrounded)
                {
                    _jumpCount = 0;
                }
            }
        }

        private void ApplyHorizontalMovementWithPrediction()
        {
            var targetSpeed = (_runHeldInput ? RunSpeed : WalkSpeed) * _moveHorizontalInput;
            var moveDelta = targetSpeed * _deltaTime;

            if (Mathf.Approximately(moveDelta, 0f))
            {
                _velocity.x = 0f;
                return;
            }

            var direction = Mathf.Sign(moveDelta);
            var rayLength = Mathf.Abs(moveDelta) + SkinWidth;
            
            var bounds = Collider2D.bounds;
            var spacing = (bounds.size.y - 2f * SkinWidth) / (WallRayCount - 1);
            var rayOriginX = direction > 0 ? bounds.max.x : bounds.min.x;
    
            var shortestDistance = Mathf.Abs(moveDelta);

            for (var i = 0; i < WallRayCount; i++)
            {
                var origin = new Vector2(rayOriginX, bounds.min.y + SkinWidth + i * spacing);
                if (_drawDebug)
                    Debug.DrawRay(origin, Vector2.right * (direction * rayLength), Color.magenta);

                var hit = Physics2D.Raycast(origin, Vector2.right * direction, rayLength, GroundMask);
                if (hit)
                {
                    var available = hit.distance - SkinWidth - CollisionFudge;
                    shortestDistance = Mathf.Min(shortestDistance, Mathf.Max(0f, available));
                }
            }

            var actualMove = direction * shortestDistance;
            transform.position += new Vector3(actualMove, 0f, 0f);
            
            //Detect actual overlap
            var finalBounds = Collider2D.bounds;
            var wallCheckOrigin = new Vector2(direction > 0 ? finalBounds.max.x : finalBounds.min.x, finalBounds.center.y);
            var overlapRayLength = CollisionFudge * 2f;
            var overlap = Physics2D.Raycast(wallCheckOrigin, Vector2.right * direction, overlapRayLength, GroundMask);

            if (overlap && overlap.distance < CollisionFudge)
            {
                //Snap back
                var correction = CollisionFudge - overlap.distance;
                transform.position += new Vector3(-direction * correction, 0f, 0f);
                _velocity.x = 0f;
                return;
            }
            
            _velocity.x = actualMove / _deltaTime;
        }

        private void ApplyGravity()
        {
            if (!_grounded || _velocity.y > 0f)
            {
                _velocity.y += _calculatedGravity * _deltaTime;
                _velocity.y = Mathf.Max(_velocity.y, MaxFallSpeed);
            }

            if (_grounded)
            {
                _velocity.y = 0;
            }
            
            if (!_jumpHeldInput && _velocity.y > 0f)
            {
                _velocity.y *= 0.5f;
            }
        }

        private bool IsWithinCoyoteTime => Time.time < _lastGroundedTime + CoyoteTime;
        
        private void HandleJump()
        {
            var bufferedJump = Time.time < _lastJumpRequestedTime + JumpBufferTime;

            if (bufferedJump && (_grounded || IsWithinCoyoteTime || _jumpCount < MaxJumps))
            {
                _velocity.y = _calculatedJumpForce;
                _jumpCount++;
                _lastJumpRequestedTime = -999f;
            }
        }
        
        private void HandleDash()
        {
            if (_dashing && Time.time > _dashEndTime)
            {
                _dashing = false;
            }

            if (_dashRequested && !_dashing && Time.time >= _lastDashTime + DashCooldown)
            {
                _dashing = true;
                _dashEndTime = Time.time + DashDuration;
                _lastDashTime = Time.time;
                _velocity = _dashDirection * DashForce;
            }
        }

        #endregion
    }
}