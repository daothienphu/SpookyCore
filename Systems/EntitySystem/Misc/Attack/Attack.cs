using System;
using System.Collections.Generic;

namespace SpookyCore.EntitySystem
{
    public class Attack
    {
        #region Enum

        [Serializable]
        public enum AttackState
        {
            Running,
            Finished,
            Stopped,
        }

        #endregion
        
        #region Fields

        private readonly EntityAttack _owner;
        private readonly List<IAttackPhase> _phases;
        private int _currentPhaseIndex;
        private Entity _target;

        #endregion
        
        #region Properties
        
        public AttackState State { get; private set; }

        #endregion

        #region Life Cycle

        public Attack(EntityAttack owner, List<IAttackPhase> phases)
        {
            _owner = owner;
            _phases = phases;
        }

        #endregion
        
        #region Public Methods
        
        public void Start(Entity target)
        {
            _currentPhaseIndex = 0;
            
            if (_phases.Count <= 0)
            {
                State = AttackState.Finished;
                return;
            }

            _target = target;
            _phases[_currentPhaseIndex].StartPhase(_owner, _target);
            State = AttackState.Running;
        }
        
        public void Update()
        {
            if (State is AttackState.Finished or AttackState.Stopped || _phases.Count == 0) return;
            
            var isPhaseDone = _phases[_currentPhaseIndex].UpdatePhase(_owner);
            
            if (!isPhaseDone) return;
            
            _currentPhaseIndex++;
            if (_currentPhaseIndex < _phases.Count)
            {
                _phases[_currentPhaseIndex].StartPhase(_owner, _target);
            }
            else
            {
                State = AttackState.Finished;
                _target = null;
            }
        }

        public void Stop()
        {
            if (_currentPhaseIndex < _phases.Count)
            {
                _phases[_currentPhaseIndex].StopPhase(_owner);
            }
            State = AttackState.Stopped;
            _target = null;
        }
        
        #endregion
    }
}