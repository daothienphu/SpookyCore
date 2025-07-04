using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class ColliderListener : MonoBehaviour
    {
        public EntityCollider ParentEntityCollider;
        public EntityTrigger ParentEntityTrigger;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ParentEntityCollider?.RegisterCollisionEnter(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            ParentEntityCollider?.RegisterCollisionExit(collision);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            ParentEntityTrigger?.RegisterTriggerEnter(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ParentEntityTrigger?.RegisterTriggerExit(other);
        }
    }
}