using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityAnimationRunner : EntityComponentBase
    {
        #region Fields

        public Animator Animator;
        private readonly Dictionary<Type, BaseAnimationState> _animationCache = new();
        private BaseAnimationState _currentAnimationState;

        #endregion

        #region Life Cycle

        public override void OnStart()
        {
            if (!Animator)
            {
                Animator = GetComponent<Animator>();
            }
            if (!Animator)
            {
                Debug.LogWarning($"Animator missing on {Entity.ID}");
            }
        }

        public override void OnUpdate()
        {
            if (!Animator) return;
            _currentAnimationState?.Update();
            if (_currentAnimationState is { State: BaseAnimationState.AnimationState.Exited })
            {
                _currentAnimationState = null;
            }
        }

        #endregion

        #region Public Methods

        public void PlayAnimation(Type animType, float transitionTime = 0.1f, Action onEnd = null)
        {
            if (!Animator) return;
            
            if (!typeof(IAnimationState).IsAssignableFrom(animType))
            {
                Debug.LogError($"Type {animType} is not a valid animation state.");
                return;
            }

            if (!_animationCache.TryGetValue(animType, out var animationState))
            {
                animationState = (BaseAnimationState)Activator.CreateInstance(animType);
                _animationCache[animType] = animationState;
            }
            animationState.RegisterOnExitCallback(onEnd);
            
            
            _currentAnimationState?.Exit();
            _currentAnimationState = animationState;
            _currentAnimationState.Enter(Entity, transitionTime);
        }
        
        public void StopCurrentAnimation()
        {
            _currentAnimationState?.Exit();
            _currentAnimationState = null;
        }
        
        public bool IsAnimationPlaying(Type animType)
        {
            return _animationCache.TryGetValue(animType, out var animationState) && 
                   animationState == _currentAnimationState;
        }

        public void SetAnimatorSpeed(float speed)
        {
            if (!Animator) return;
            Animator.speed = speed;
        }

        #endregion
    }
}