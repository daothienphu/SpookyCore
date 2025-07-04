using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    [RequireComponent(typeof(Animator), typeof(EntityVisual))]
    public class EntityAnimation : EntityComponent
    {
        #region Fields

        [SerializeField] public EntityAnimationConfig AnimationConfig;
        [SerializeField] internal RuntimeAnimatorController _animatorController;

        private Animator _animator;
        private Queue<(EntityAnimState state, Action onBefore, Action onAfter)> _queue = new();

        private EntityVisual _visual;
        private EntityAnimState _currentState = EntityAnimState.None;
        private EntityAnimState _defaultState;
        private float _currentClipLength;
        private float _timer;
        private bool _isPlaying;
        private bool _isCurrentAnimationLooped;

        private Action _onAfterCurrentAnimation;

        #endregion

        #region Life Cycle

        public override void OnAwake()
        {
            _visual = Entity.Get<EntityVisual>();
            _animator = GetComponent<Animator>();
            if (_animatorController)
            {
                _animator.runtimeAnimatorController = _animatorController;
            }

            _queue = new Queue<(EntityAnimState state, Action onBefore, Action onAfter)>();

            var defaultClipName = AnimationConfig.AnimationClips[0].name;
            _defaultState = defaultClipName.ToAnimationState(Entity.ID);
        }

        public override void OnStart()
        {
            PlayAnimation(_defaultState);
        }

        public override void OnUpdate()
        {
            if (!_isPlaying || _isCurrentAnimationLooped)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= _currentClipLength)
            {
                _isPlaying = false;
                _onAfterCurrentAnimation?.Invoke();
                _onAfterCurrentAnimation = null;
                _currentState = EntityAnimState.None;
                PlayNextInQueue();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called in the Editor only to initialize the Animator and preview sprite.
        /// </summary>
        public void InitAnimatorController(RuntimeAnimatorController controller, Sprite previewSprite)
        {
            if (!_visual)
            {
                _visual = GetComponent<EntityVisual>();
            }

            if (_visual && previewSprite)
            {
                _visual.MainVisualRenderer.sprite = previewSprite;
            }

            _animatorController = controller;

            if (!_animator)
            {
                _animator = GetComponent<Animator>();
            }

            if (_animator)
            {
                _animator.runtimeAnimatorController = controller;
            }
        }

        public virtual void PlayAnimation(string stateString, Action onBeforePlay = null, Action onAfterPlay = null)
        {
            var state = stateString.ToAnimationState(Entity.ID);
            PlayAnimation(state, onBeforePlay, onAfterPlay);
        }
        
        public virtual void PlayAnimation(EntityAnimState state, Action onBeforePlay = null, Action onAfterPlay = null)
        {
            if (_currentState == state && _isPlaying)
            {
                return;
            }

            onBeforePlay?.Invoke();

            var stateName = state.ToAnimationString();
            _animator.Play(stateName, 0, 0f);

            _currentState = state;
            _currentClipLength = GetClipLength(stateName);
            _timer = 0f;
            _isPlaying = true;
            _onAfterCurrentAnimation = onAfterPlay;
        }

        public virtual void QueueAnimation(EntityAnimState state, Action onBeforePlay = null, Action onAfterPlay = null)
        {
            _queue.Enqueue((state, onBeforePlay, onAfterPlay));

            if (!_isPlaying)
            {
                PlayNextInQueue();
            }
        }

        public virtual void StopCurrentAnimation()
        {
            _queue.Clear();
            PlayAnimation(_defaultState);
        }

        public virtual bool IsAnimationPlaying(string stateString)
        {
            var state = stateString.ToAnimationState(Entity.ID);
            return IsAnimationPlaying(state);
        }
        
        public virtual bool IsAnimationPlaying(EntityAnimState state)
        {
            return _isPlaying && _currentState == state;
        }

        #endregion

        #region Private Methods

        protected virtual float GetClipLength(string clipName)
        {
            foreach (var clip in _animatorController.animationClips)
            {
                if (clip.name != clipName) continue;
                
                _isCurrentAnimationLooped = clip.isLooping;
                return clip.length;
            }

            return 0f;
        }

        protected virtual void PlayNextInQueue()
        {
            if (_queue.Count == 0)
            {
                if (Entity.State == Entity.EntityState.Alive)
                {
                    PlayAnimation(_defaultState);
                }
                return;
            }

            var (state, before, after) = _queue.Dequeue();
            PlayAnimation(state, before, after);
        }

        #endregion
    }
}