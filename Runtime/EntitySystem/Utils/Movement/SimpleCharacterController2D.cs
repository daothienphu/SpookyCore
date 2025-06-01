using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class SimpleCharacterController2D : MonoBehaviour
    {
        #region Fields
        
        // Config
        public float WalkSpeed = 5f;
        public float RunSpeed = 8f;
        public float Acceleration = 50f;
        public float Deceleration = 40f;

        public float JumpForce = 12f;
        public int MaxJumps = 2;

        public float DashForce = 20f;
        public float DashDuration = 0.2f;
        public float DashCooldown = 0.5f;

        public float Gravity = -30f;

        // Inputs
        private float _moveInput;
        private bool _runInput;
        private bool _jumpRequested;
        private bool _dashRequested;
        private Vector2 _dashDirection;

        // Internal State
        private int _jumpCount = 0;
        private bool _wasGroundedLastFrame = false;
        private float _dashEndTime = -999f;
        private float _lastDashTime = -999f;
        private float _deltaTime;

        private Vector2 _velocity;
        private bool _grounded;
        private bool _dashing;

        #endregion

        #region Properties

        // === Exposed Properties ===
        public Vector2 Velocity => _velocity;
        public bool IsGrounded => _grounded;
        public bool IsDashing => _dashing;
        public bool IsJumping => !_grounded && _velocity.y > 0f;
        public bool IsFalling => !_grounded && _velocity.y < 0f;

        #endregion

        #region Life Cycle

        public void Tick(float dt)
        {
            _deltaTime = dt;

            HandleDash();
            if (!_dashing)
            {
                ApplyHorizontalMovement();
                ApplyGravity();
                HandleJump();
            }

            _jumpRequested = false;
            _dashRequested = false;
        }

        #endregion
        
        #region Public Methods

        public void SetInputs(float horizontal, bool run)
        {
            _moveInput = horizontal;
            _runInput = run;
        }

        public void RequestJump()
        {
            _jumpRequested = true;
        }
        
        public void RequestDash(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f) direction = Vector2.right;
            _dashRequested = true;
            _dashDirection = direction.normalized;
        }
        
        public void SetGrounded(bool isGrounded)
        {
            _grounded = isGrounded;
            if (_grounded && !_wasGroundedLastFrame)
            {
                _jumpCount = 0;
            }

            _wasGroundedLastFrame = _grounded;
        }

        #endregion

        #region Private Methods

        private void ApplyHorizontalMovement()
        {
            var targetSpeed = (_runInput ? RunSpeed : WalkSpeed) * _moveInput;
            var speedDiff = targetSpeed - _velocity.x;
            var accelRate = Mathf.Abs(targetSpeed) > 0.01f ? Acceleration : Deceleration;

            _velocity.x += accelRate * speedDiff * _deltaTime;
        }

        private void ApplyGravity()
        {
            _velocity.y += Gravity * _deltaTime;
        }

        private void HandleJump()
        {
            if (_jumpRequested && (_grounded || _jumpCount < MaxJumps - 1))
            {
                _velocity.y = JumpForce;
                _jumpCount++;
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