using UnityEngine;

namespace SpookyCore.EntitySystem.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class ColliderListener : MonoBehaviour
    {
        public EntityCollider ParentEntityCollider;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (ParentEntityCollider)
            {
                ParentEntityCollider.RegisterCollisionEnter(collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (ParentEntityCollider)
            {
                ParentEntityCollider.RegisterCollisionExit(collision);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (ParentEntityCollider)
            {
                ParentEntityCollider.RegisterTriggerEnter(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (ParentEntityCollider)
            {
                ParentEntityCollider.RegisterTriggerExit(other);
            }
        }
    }
}