using SpookyCore.Utilities;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class BodyCollider : MonoBehaviour
    {
        public bool HasCollidedWithSomething;
        public EntityBase CollidedEntity;
        
        private void Start()
        {
            HasCollidedWithSomething = false;
            CollidedEntity = null;
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (other.IsCollisionTarget(SpookyCoreExtensions.ColliderTarget.AnyEntities | 
                                        SpookyCoreExtensions.ColliderTarget.Ground))
            {
                HasCollidedWithSomething = true;
                other.TryGetEntity(out CollidedEntity);
            }
        }

        protected virtual void OnCollisionExit2D(Collision2D other)
        {
            if (other.IsCollisionTarget(SpookyCoreExtensions.ColliderTarget.AnyEntities |
                                        SpookyCoreExtensions.ColliderTarget.Ground))
            {
                HasCollidedWithSomething = false;
                CollidedEntity = null;
            }
        }
    }
}