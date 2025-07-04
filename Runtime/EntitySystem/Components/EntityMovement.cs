using SpookyCore.Runtime.Utilities;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class EntityMovement : EntityComponent
    {
        #region Fields

        [Header("General Settings")]
        [field: SerializeField] public bool IsEnabled { get; protected set; } = true;
        [field: SerializeField] public Vector2 Velocity { get; protected set; }
        [field: SerializeField] public bool IsHeadingTowardsDestination { get; protected set; }
        [field: SerializeField] public Vector2 Destination { get; protected set; }
        [field: SerializeField] public bool IsFollowingTarget { get; protected set; }
        [field: SerializeField] public Transform TargetToFollow { get; protected set; }
        
        [SerializeField] protected float _movementSpeed = 5f;
        [SerializeField] protected bool _setAnimationBasedOnMovement;
        protected EntityAnimation _animation;
        
        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            _animation = Entity.Get<EntityAnimation>();
        }

        public override void OnFixedUpdate()
        {
            if (!IsEnabled) return;

            if (IsFollowingTarget)
            {
                HeadTowardTarget(TargetToFollow);
            }

            if (IsHeadingTowardsDestination)
            {
                transform.Translate(Velocity * Time.fixedDeltaTime);
            }

            if (_setAnimationBasedOnMovement && (IsFollowingTarget || IsHeadingTowardsDestination))
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

        public virtual void HeadTowardPosition(Vector3 position)
        {
            StopAllMovement();
            IsHeadingTowardsDestination = true;
            Destination = position;
            Velocity = (Destination - transform.position.V2()).normalized * _movementSpeed;
        }

        public virtual void FollowTarget(Transform target)
        {
            StopAllMovement();
            TargetToFollow = target;
            IsFollowingTarget = true;
        }
        
        public virtual void Move(Vector2 deltaMovement, float deltaTime = 0)
        {
            StopAllMovement();
            transform.Translate(deltaMovement);
            
            Velocity = deltaMovement / Time.fixedDeltaTime;
        }

        public virtual void StopAllMovement()
        {
            //Velocity = Vector2.zero;
            TargetToFollow = null;
            IsHeadingTowardsDestination = false;
            IsFollowingTarget = false;
        }

        #endregion

        #region Private Methods
        
        protected virtual void HeadTowardTarget(Transform target)
        {
            if (!target) return;
            Velocity = (target.position - transform.position).normalized * _movementSpeed;
        }
        
        protected virtual void HandleAnimation()
        {
            if (!_animation)
            {
                return;
            }
            
            var state = "Idle";
            if (Velocity.sqrMagnitude > 0.01f)
            {
                state = "Walk";
            }
            
            if (!_animation.IsAnimationPlaying(state))
            {
                _animation.PlayAnimation(state);
            }
        }

        #endregion
    }
}