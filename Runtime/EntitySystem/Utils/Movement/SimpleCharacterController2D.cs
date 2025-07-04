using SpookyCore.Runtime.Utilities;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class SimpleCharacterController2D : KinematicObjectMovement
    {
        #region Fields

        public Vector2 Velocity;
        
        [Header("Horizontal Movement Settings (Overrides EntityMovement)")]
        public float WalkSpeed = 4f;
        public float RunSpeed = 6f;
        
        [Header("Jump Settings")]
        public float DesiredJumpHeight = 2f;
        public float TimeToApex = 0.3f;
        public int MaxJumps = 2;
        [SerializeField] private float MaxFallSpeed = -20f;
        [SerializeField] private float CoyoteTime = 0.1f;
        [SerializeField] private float JumpBufferTime = 0.1f;
        
        // [Header("Dash Settings")]
        // public float DashForce = 20f;
        // public float DashDuration = 0.2f;
        // public float DashCooldown = 0.5f;
        
        [Header("Inputs")]
        [ReadOnly, SerializeField] private float _moveHorizontalInput;
        [ReadOnly, SerializeField] private bool _runHeldInput;
        [ReadOnly, SerializeField] private bool _jumpHeldInput;
        [ReadOnly, SerializeField] private float _lastJumpRequestedTime = -999f;
        //[ReadOnly, SerializeField] private bool _dashRequested;
        //[ReadOnly, SerializeField] private Vector2 _dashDirection;

        [Header("Internal States")]
        [ReadOnly, SerializeField] private float _calculatedGravity;
        [ReadOnly, SerializeField] private float _calculatedJumpForce;
        [ReadOnly, SerializeField] private int _jumpCount;
        [ReadOnly, SerializeField] private float _lastGroundedTime;
        //[ReadOnly, SerializeField] private bool _dashing;
        //[ReadOnly, SerializeField] private float _dashEndTime = -999f;
        //[ReadOnly, SerializeField] private float _lastDashTime = -999f;
        
        #endregion

        #region Properties
        
        public bool IsGrounded => Collisions.Below;
        //public bool IsDashing => _dashing;
        public bool IsJumping => !Collisions.Below && Velocity.y > 0f;
        public bool IsFalling => !Collisions.Below && Velocity.y < 0f;
        private bool IsWithinCoyoteTime => Time.time < _lastGroundedTime + CoyoteTime;

        #endregion

        #region Life Cycle

        private void Awake()
        {
            CalculatePhysicsConstants();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            CalculatePhysicsConstants();
        }
#endif

        public void Tick(float dt)
        {
            _deltaTime = dt;
            
            ApplyHorizontalVelocity(ref Velocity);
            ApplyGravityVelocity(ref Velocity);
            ApplyJumpVelocity(ref Velocity);
            
            Velocity = Move(Velocity * _deltaTime) / _deltaTime;
        }

        #endregion
        
        #region Public Methods
        
        public void SetInputs(float horizontal, bool run)
        {
            _moveHorizontalInput = horizontal;
            _runHeldInput = run;
        }

        public void SetJumpState(bool held)
        {
            if (held && !_jumpHeldInput)
            {
                _lastJumpRequestedTime = Time.time;
            }
            _jumpHeldInput = held;
        }
        
        public void RequestDash(Vector2 direction)
        {
            // if (direction.sqrMagnitude < 0.001f)
            // {
            //     direction = Vector2.right;
            // }
            // _dashRequested = true;
            // _dashDirection = direction.normalized;
        }

        #endregion

        #region Private Methods
        
        private void CalculatePhysicsConstants()
        {
            _calculatedGravity = -(2 * DesiredJumpHeight) / (TimeToApex * TimeToApex);
            _calculatedJumpForce = Mathf.Abs(_calculatedGravity) * TimeToApex;
        }

        private void ApplyHorizontalVelocity(ref Vector2 velocity)
        {
            velocity.x = (_runHeldInput ? RunSpeed : WalkSpeed) * _moveHorizontalInput;
        }
        
        private void ApplyGravityVelocity(ref Vector2 velocity)
        {
            if (!Collisions.Below)
            {
                velocity.y += _calculatedGravity * _deltaTime;
                velocity.y = Mathf.Max(velocity.y, MaxFallSpeed);
            } 
            else
            {
                velocity.y = 0;
            }
            
            if (!_jumpHeldInput && velocity.y > 0f)
            {
                velocity.y *= 0.5f;
            }
        }

        private void ApplyJumpVelocity(ref Vector2 velocity)
        {
            var bufferedJump = Time.time < _lastJumpRequestedTime + JumpBufferTime;
            
            if (bufferedJump && (Collisions.Below || IsWithinCoyoteTime || _jumpCount < MaxJumps))
            {
                velocity.y = _calculatedJumpForce;
                _jumpCount++;
                _lastJumpRequestedTime = -999f;
            }
        }
        
        // private Vector2 HandleDash()
        // {
        //     if (_dashing && Time.time > _dashEndTime)
        //     {
        //         _dashing = false;
        //     }
        //
        //     if (_dashRequested && !_dashing && Time.time >= _lastDashTime + DashCooldown)
        //     {
        //         _dashing = true;
        //         _dashEndTime = Time.time + DashDuration;
        //         _lastDashTime = Time.time;
        //         return _dashDirection * DashForce;
        //     }
        //     
        //     return Vector2.zero;
        // }

        #endregion
    }
}