using System;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public abstract class BaseAnimationState : IAnimationState
    {
        #region Enum

        [Serializable]
        public enum AnimationState
        {
            Entered,
            Running,
            Exited,
        }

        #endregion

        #region Fields

        protected Entity Entity;
        protected Animator Animator;
        protected string AnimationName;
        private Action _onExitCallback;
        private float _startTime;
        private float _length;
        private bool _isLoop;

        #endregion

        #region Properties
        
        public AnimationState State { get; protected set; }
        
        #endregion

        #region Public Methods

        public virtual void Enter(Entity entity, float transitionTime)
        {
            if (AnimationName == "")
            {
                Debug.Log($"Unassigned animation name for {GetType()}");
                State = AnimationState.Exited;
                return;
            }
            
            State = AnimationState.Entered;
            
            Entity = entity;
            Animator = Entity.Get<EntityAnimationRunner>().Animator;
            
            Animator.CrossFade(AnimationName, transitionTime);
            _startTime = Time.time + transitionTime;
            (_length, _isLoop) = GetAnimationLength();

            State = AnimationState.Running;
        }

        public virtual void Update()
        {
            if (!Animator) return;
  
            if (!_isLoop && Time.time >= _startTime + _length)
            {
                Exit();
            }
        }

        public virtual void Exit()
        {
            State = AnimationState.Exited;
            _onExitCallback?.Invoke();
            Animator = null;
        }

        public virtual void RegisterOnExitCallback(Action onExitCallback)
        {
            _onExitCallback = onExitCallback;
        }

        #endregion

        #region Private Methods
        
        private (float, bool) GetAnimationLength()
        {
            if (!Animator) return (0f, false);

            foreach (var clip in Animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == AnimationName)
                {
                    return (clip.length, clip.wrapMode == WrapMode.Loop);
                }
            }
            
            return (0f, false);
        }

        #endregion
    }
}