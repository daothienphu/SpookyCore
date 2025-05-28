using UnityEngine;
using System;

namespace SpookyCore.EntitySystem
{
    [Serializable]
    public abstract class EntityComponent : MonoBehaviour
    {
        public Entity Entity { get; private set; }

        public void Init(Entity entity)
        {
            Entity = entity;
        }
        
        public virtual void OnAwake(){ }

        public virtual void OnStart() { }

        public virtual void OnUpdate() { }

        public virtual void OnFixedUpdate() { }

        public virtual void OnBeforeDead() { }

        public virtual void OnAfterDead() { }
    }
}