using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityStatusEffectReceiver : EntityComponent
    {
        #region Fields

        private readonly List<StatusEffectHandlerBase> _activeEffects = new();
        private readonly List<StatusEffectHandlerBase> _effectHandlersCache = new();

        #endregion

        #region Life Cycle

        public override void OnUpdate()
        {
            for (var i = _activeEffects.Count - 1; i >= 0; i--)
            {
                var isExpired = _activeEffects[i].UpdateEffect(Time.deltaTime);
                if (isExpired)
                {
                    _activeEffects[i].RemoveEffect();
                    _activeEffects.RemoveAt(i);
                }
            }
        }

        #endregion
        
        #region Public Methods

        public void ApplyEffect(StatusEffectConfig statusEffectConfig)
        {
            //Effect is active -> reapply
            foreach (var effect in _activeEffects)
            {
                if (effect.Config == statusEffectConfig)
                {
                    effect.ApplyEffect(this);
                    return;
                }
            }
            
            StatusEffectHandlerBase effectHandlerInstance = null;
            //Effect was cached
            foreach (var effectHandler in _effectHandlersCache)
            {
                if (effectHandler.Config == statusEffectConfig)
                {
                    effectHandlerInstance = effectHandler;
                    break;
                }
            }
            
            //Create new instance
            if (effectHandlerInstance == null)
            {
                effectHandlerInstance = statusEffectConfig.CreateEffectHandler();
                _effectHandlersCache.Add(effectHandlerInstance);
            }
            
            _activeEffects.Add(effectHandlerInstance);
            effectHandlerInstance.ApplyEffect(this);
        }

        public void RemoveEffect(StatusEffectConfig statusEffectConfig)
        {
            foreach (var effect in _activeEffects)
            {
                if (effect.Config == statusEffectConfig)
                {
                    effect.RemoveEffect();
                    _activeEffects.Remove(effect);
                    break;
                }
            }
        }

        public bool HasEffect<T>() where T : StatusEffectConfig
        {
            return _activeEffects.Exists(effect => effect.Config is T);
        }
        
        #endregion
    }
}