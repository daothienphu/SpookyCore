using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityVisual : EntityComponent
    {
        #region Fields

        public Transform MainVisualTransform;
        public SpriteRenderer MainVisualRenderer;
        [SerializeField] protected bool _flipXBasedOnVelocity;
        
        public bool IsFacingRight = true;
        
        protected EntityMovement _movement;
        
        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            _movement = Entity.Get<EntityMovement>();
        }

        public override void OnStart()
        {
            if (!MainVisualTransform)
            {
                Debug.LogError($"Main Visual Transform unassigned at {name}");
            }

            if (!MainVisualRenderer)
            {
                Debug.LogError($"Main Visual Renderer unassigned at {name}");
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