using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class EntityMovement : EntityComponent
    {
        #region Fields

        [field: SerializeField] public Vector2 Velocity { get; private set; }
        
        [field: SerializeField] public bool IsEnabled { get; set; } = true;
        [SerializeField] protected bool IsManualMovement;
        
        [Header("Default General Settings")]
        [SerializeField] protected bool _alsoSetAnimation;
        [SerializeField] protected bool _usePlatformerMovementSet;
        public SimpleCharacterController2D CharacterController;

        [Header("Default Movement Settings")]
        [SerializeField] protected float _walkSpeed = 5f;
        [SerializeField] protected float _runMultiplier = 1.5f;
        [SerializeField] protected float _jumpForce = 7f;
        
        protected EntityInputReceiver _input;
        protected EntityAnimation _animation;
        protected EntityCollider _collider;
        protected EntityVisual _visual;

        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            _input = Entity.Get<EntityInputReceiver>();
            _animation = Entity.Get<EntityAnimation>();
            _collider = Entity.Get<EntityCollider>();
                
            if (_input && _input.enabled) IsManualMovement = true;
        }

        public override void OnStart()
        {
            if (_usePlatformerMovementSet)
            {
                CharacterController.Collider2D = _collider.Collider2D;
            }
        }

        public override void OnUpdate()
        {
            if (!IsEnabled) return;

            if (_usePlatformerMovementSet)
            {
                CharacterController.SetInputs(_input.MoveInput.x, _input.RunHeld);
                CharacterController.RequestJump(_input.JumpHeld);
                if (_input.DashPressed) CharacterController.RequestDash(_input.MoveInput.normalized);
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
            
            if (_alsoSetAnimation)
            {
                HandleAnimation();
            }
        }

        #endregion

        #region Public Methods

        public virtual void ToggleMovement(bool canMove)
        {
            IsEnabled = canMove;
        }
        
        /// <summary>
        /// Move using the movement specified.
        /// </summary>
        /// <param name="movement">The movement to be added in this frame.</param>
        /// <param name="isDeltaMovement">True if this movement has been premultiplied with the respective deltaTime.</param>
        public virtual void Move(Vector2 movement, bool isDeltaMovement)
        {
            
        }

        #endregion

        #region Private Methods

        protected virtual void HandleGeneralMovement(float deltaTime)
        {
            var movement = new Vector2(
                _input.MoveInput.x,
                _input._verticalMovementEnabled ? _input.MoveInput.y : 0);

            movement = movement.normalized;
            
            var speed = _walkSpeed * (_input.RunHeld ? _runMultiplier : 1f);

            movement *= speed;
            
            if (_input._canJump && _input.JumpPressed)
            {
                movement.y = _jumpForce;
                _input.ResetJump();
            }
            
            Velocity = movement;
            transform.Translate(movement * deltaTime);
        }

        protected virtual void HandlePlatformerMovement(float deltaTime)
        {
            CharacterController.Tick(deltaTime);
            
            Velocity = CharacterController.Velocity;
            transform.Translate(Velocity * deltaTime);
        }

        protected virtual void HandleAnimation()
        {
            if (!_animation)
            {
                return;
            }

            var state = "Idle";
            
            if ((_alsoSetAnimation && Mathf.Abs(Velocity.x) > 0.001f) || (!_alsoSetAnimation && Velocity.sqrMagnitude > 0.001f))
            {
                state = _input.RunHeld ? "Run" : "Walk";
            }

            if (!_animation.IsAnimationPlaying(state))
            {
                _animation.PlayAnimation(state);
            }
        }

        #endregion
    }
}