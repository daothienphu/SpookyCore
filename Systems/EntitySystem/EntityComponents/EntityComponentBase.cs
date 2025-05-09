using UnityEngine;
using System;

namespace SpookyCore.EntitySystem
{
    [Serializable]
    public abstract class EntityComponentBase : MonoBehaviour
    {
        public EntityBase Entity { get; private set; }

        public void Init(EntityBase entity)
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