using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntitySequenceRunner : EntityComponent
    {
        #region Fields

        [SerializeField] private List<ScriptedSequence> _sequences;
        private ScriptedSequence _currentBaseScriptedSequence;
        
        private Action _onStartCallback;
        private Action _onEndCallback;
        private Action _onInterruptCallback;
        private Func<bool> _interruptCondition;

        #endregion

        #region Life Cycle

        public override void OnStart()
        {
            //Clones a version of the scriptable object to avoid data sharing between entities with the same sequences
            for (var i = 0; i < _sequences.Count; i++)
            {
                var sequence = _sequences[i];
                _sequences[i] = sequence.Clone();
            }
        }

        public override void OnUpdate()
        {
            if (!_currentBaseScriptedSequence) return;

            if (_interruptCondition?.Invoke() ?? false)
            {
                InterruptCurrentSequence();
                return;
            }

            if (_currentBaseScriptedSequence?.UpdateSequence(Time.deltaTime) ?? false)
            {
                EndCurrentSequence();
            }
        }

        #endregion

        #region Public Methods

        public void StartSequence(
            SequenceID sequenceID,
            Action onStart = null,
            Action onEnd = null,
            Func<bool> interruptCondition = null,
            Action onInterrupt = null,
            BaseSequenceContext ctx = null)
        {
            if (_currentBaseScriptedSequence && _currentBaseScriptedSequence.SequenceID == sequenceID)
            {
                Debug.Log($"Trying to start sequence {sequenceID} again in entity {Entity.gameObject.name}.");
                return;
            }

            var i = FindSequenceIndex(sequenceID);

            if (i == -1)
            {
                Debug.Log($"Can't find sequence {sequenceID} in entity {Entity.gameObject.name}.");
                return;
            }

            if (_currentBaseScriptedSequence && _currentBaseScriptedSequence.SequenceID != sequenceID)
            {
                Debug.Log($"Interrupting sequence {_currentBaseScriptedSequence.SequenceID} with {sequenceID} in entity {Entity.gameObject.name}");
                InterruptCurrentSequence();
            }
            
            _onStartCallback = onStart;
            _onEndCallback = onEnd;
            _onInterruptCallback = onInterrupt;
            _interruptCondition = interruptCondition;
            
            _currentBaseScriptedSequence = _sequences[i];
            _currentBaseScriptedSequence.Initialize(this, ctx);
            _currentBaseScriptedSequence.OnStart();
            Debug.Log($"{_currentBaseScriptedSequence.SequenceID} start callback");
            _onStartCallback?.Invoke();
        }

        #endregion

        #region Private Methods

        private void InterruptCurrentSequence()
        {
            if (!_currentBaseScriptedSequence) return;
            
            _currentBaseScriptedSequence.OnInterrupted();
            Debug.Log($"{_currentBaseScriptedSequence.SequenceID} interrupt callback");
            _currentBaseScriptedSequence = null;
            _onInterruptCallback?.Invoke();
        }

        private void EndCurrentSequence()
        {
            if (!_currentBaseScriptedSequence) return;
            
            _currentBaseScriptedSequence.OnEnd();
            Debug.Log($"{_currentBaseScriptedSequence.SequenceID} end callback");
            _currentBaseScriptedSequence = null;
            _onEndCallback?.Invoke();
        }

        private int FindSequenceIndex(SequenceID sequenceID)
        {
            var i = 0;
            while (i < _sequences.Count)
            {
                if (_sequences[i].SequenceID == sequenceID)
                {
                    break;
                }
                i++;
            }

            return i < _sequences.Count ? i : -1;
        }

        #endregion
    }
}