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

        protected bool TryGetEntity(Transform fromTransform, out EntityBase entityBase)
        {
            if (!fromTransform)
            {
                entityBase = null;
                return false;
            }
            
            if (fromTransform.CompareTag(GameTags.IgnoredByTriggers))
            {
                entityBase = null;
                return false;
            }
            
            return fromTransform.TryGetComponent(out entityBase) || 
                   (fromTransform.parent && fromTransform.parent.TryGetComponent(out entityBase)) || 
                   (fromTransform.parent.parent && fromTransform.parent.parent.TryGetComponent(out entityBase));
        }
    }
}