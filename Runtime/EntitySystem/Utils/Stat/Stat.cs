using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [Serializable]
    public class Stat : ISerializationCallbackReceiver
    {
        public float Base;
        //Hide this in the inspector of EntityStatConfig
        [HideInInspector] public float Current;
        private readonly List<IStatModifier> _modifiers = new();
        
        public void AddModifier(IStatModifier modifier)
        {
            _modifiers.Add(modifier);
            Recalculate();
        }

        public void RemoveModifier(IStatModifier modifier)
        {
            _modifiers.Remove(modifier);
            Recalculate();
        }

        protected virtual void Recalculate()
        {
            Current = Base;
            foreach (var mod in _modifiers)
            {
                mod.Apply(ref Current);
            }
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            Current = Base;
        }
    }
}