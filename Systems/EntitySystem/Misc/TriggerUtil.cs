using SpookyCore.SystemLoader;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public abstract class TriggerUtil : MonoBehaviour
    {
        protected IEventBus EventBus;
        
        protected virtual void Start()
        {
            EventBus = ServiceLocator.Instance.Get<IEventBus>();
        }

        protected abstract void OnTriggerEnter2D(Collider2D other);

        protected abstract void OnTriggerExit2D(Collider2D other);

        protected bool TryGetEntity(Transform fromTransform, out Entity entity)
        {
            if (!fromTransform)
            {
                entity = null;
                return false;
            }
            
            if (fromTransform.CompareTag(GameTags.IgnoredByTriggers))
            {
                entity = null;
                return false;
            }
            
            return fromTransform.TryGetComponent(out entity) || 
                   (fromTransform.parent && fromTransform.parent.TryGetComponent(out entity)) || 
                   (fromTransform.parent.parent && fromTransform.parent.parent.TryGetComponent(out entity));
        }
    }
}