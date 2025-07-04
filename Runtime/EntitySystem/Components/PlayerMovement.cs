using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class PlayerMovement : EntityMovement
    {
        #region Fields

        [Header("Player Movement Settings")]
        [SerializeField] protected bool _usePlatformerMovementSet;
        public SimpleCharacterController2D CharacterController;
        [SerializeField] protected float _runMultiplier = 1.5f;
        [SerializeField] protected float _jumpForce = 7f;
        
        protected EntityInputReceiver _input;
        protected EntityStat _stat;

        #endregion

        #region Life Cycle

        public override void OnStart()
        {
            _input = Entity.Get<EntityInputReceiver>();
            _stat = Entity.Get<EntityStat>();
        }

        public override void OnUpdate()
        {
            if (!IsEnabled) return;

            if (_usePlatformerMovementSet)
            {
                SetPlatformerInputs();
            }
        }
        
        public override void OnFixedUpdate()
        {
            if (!IsEnabled) return;
            
            if (_usePlatformerMovementSet)
            {
                HandlePlatformerMovement(Time.fixedDeltaTime);
            }
            else
            {
                HandleGeneralMovement(Time.fixedDeltaTime);
            }
            
            
            if (_setAnimationBasedOnMovement)
            {
                HandleAnimation();
            }
        }

        #endregion

        #region Public Methods

        public bool IsStandingOnPlatform()
        {
            return CharacterController.IsGrounded;
        }
        
        #endregion
        
        #region Private Methods

        protected virtual void HandleGeneralMovement(float deltaTime)
        {
            var movement = new Vector2(
                _input.MoveInput.x,
                _input._verticalMovementEnabled ? _input.MoveInput.y : 0);

            movement = movement.normalized;
            
            var speed = _movementSpeed * (_input.RunHeld ? _runMultiplier : 1f);

            movement *= speed;
            
            if (_input._canJump && _input.JumpPressed)
            {
                movement.y = _jumpForce;
                _input.ResetJump();
            }
            
            Velocity = movement;
            transform.Translate(movement * deltaTime);
        }
        
        protected virtual void SetPlatformerInputs()
        {
            CharacterController.SetInputs(_input.MoveInput.x, _input.RunHeld);
            CharacterController.SetJumpState(_input.JumpHeld);
            //if (_input.DashPressed) CharacterController.RequestDash(_input.MoveInput.normalized);
        }
        
        protected virtual void HandlePlatformerMovement(float deltaTime)
        {
            CharacterController.Tick(deltaTime);
            Velocity = CharacterController.Velocity;
        }
        
        protected override void HandleAnimation()
        {
            if (!_animation)
            {
                return;
            }
            
            var state = "Idle";
            if (_input)
            {
                if (Velocity.sqrMagnitude > 0.01f)
                {
                    state = _input.RunHeld ? "Run" : "Walk";
                }
            }
            else
            {
                if (Velocity.sqrMagnitude > 0.01f)
                {
                    state = (Velocity.magnitude >= _stat.GetStats<EntityStatConfig>().RunSpeed.Current - 0.1f) ? "Run" : "Walk";
                }
            }
            if (!_animation.IsAnimationPlaying(state))
            {
                _animation.PlayAnimation(state);
            }
            // if ((_usePlatformerMovementSet && Mathf.Abs(Velocity.x) > 0.001f) || (!_usePlatformerMovementSet && Velocity.sqrMagnitude > 0.001f))
            // {
            //     
            // }
        }

        #endregion
    }
}