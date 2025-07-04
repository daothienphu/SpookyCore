using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class EntityVisual : EntityComponent
    {
        #region Fields

        public Transform MainVisualTransform;
        [Tooltip("Useful for when your main visual only have 1 singular sprite.")]
        public SpriteRenderer MainVisualRenderer;
        [SerializeField] protected bool _flipXBasedOnVelocity;
        
        public bool IsFacingRight = true;
        
        protected EntityMovement _movement;
        
        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            _movement = Entity.Get<EntityMovement>();
            if (!_movement)
            {
                //todo: handle assignable types Get in Entity
                _movement = Entity.Get<PlayerMovement>();
            }
        }

        public override void OnStart()
        {
            if (!MainVisualTransform)
            {
                Debug.LogError($"Main Visual Transform unassigned at {name}");
            }
        }

        public override void OnUpdate()
        {
            if (_flipXBasedOnVelocity && _movement)
            {
                HandleFlipX();
            }
        }

        #endregion
        
        #region Private Methods

        protected virtual void HandleFlipX()
        {
            if ((IsFacingRight && _movement.Velocity.x < 0) ||
                (!IsFacingRight && _movement.Velocity.x > 0))
            {
                IsFacingRight = !IsFacingRight;
                var localScale = MainVisualTransform.localScale;
                localScale.x = IsFacingRight ? 1 : -1;
                MainVisualTransform.localScale = localScale;
            }
        }

        #endregion
    }
}