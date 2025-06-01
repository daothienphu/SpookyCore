using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityMovement : EntityComponent
    {
        #region Fields

        [field: SerializeField] public Vector2 Velocity { get; private set; }
        [field: SerializeField] public bool CanMove { get; set; } = true;

        [SerializeField] protected float _walkSpeed = 5f;
        [SerializeField] protected float _runMultiplier = 1.5f;
        [SerializeField] protected float _jumpForce = 7f;
        
        [SerializeField] protected bool _manualMovement;
        [SerializeField] private bool _setAnimation;
        [SerializeField] private bool _usePlatformerMovement;

        protected EntityInputReceiver _input;
        protected EntityAnimation _animation;

        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            _input = Entity.Get<EntityInputReceiver>();
            _animation = Entity.Get<EntityAnimation>();
            if (_input && _input.enabled) _manualMovement = true;
        }

        public override void OnFixedUpdate()
        {
            if (!CanMove) return;

            HandleMovement(Time.fixedDeltaTime);
            if (_setAnimation)
            {
                HandleAnimation();
            }
        }

        #endregion

        #region Public Methods

        public virtual void ToggleMovement(bool canMove)
        {
            CanMove = canMove;
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

        protected virtual void HandleMovement(float deltaTime)
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

        protected virtual void HandleAnimation()
        {
            if (!_animation)
            {
                return;
            }

            var state = "Idle";

            if (Velocity.sqrMagnitude > 0.001f)
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