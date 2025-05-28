using UnityEngine;

namespace SpookyCore.EntitySystem.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class ColliderListener : MonoBehaviour
    {
        public EntityCollider ParentCollider { get; set; }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (ParentCollider)
            {
                ParentCollider.RegisterCollisionEnter(collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (ParentCollider)
            {
                ParentCollider.RegisterCollisionExit(collision);
            }
        }
    }
}