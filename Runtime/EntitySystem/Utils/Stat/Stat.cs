using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.EntitySystem.Utils.Stat
{
    [CreateAssetMenu(menuName = "SpookyCore/Components/Stat/StatEntry", fileName = "StatEntry")]
    public class Stat : ScriptableObject
    {
        [SerializeField] private float _baseValue;
        private float _currentValue;
        private readonly List<IStatModifier> _modifiers = new();

        public float BaseValue => _baseValue;
        public float CurrentValue => _currentValue;

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
            _currentValue = _baseValue;
            foreach (var mod in _modifiers)
            {
                mod.Apply(ref _currentValue);
            }
        }
    }
}