using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.Runtime.Systems
{
    public class TimerSystem : PersistentMonoSingleton<TimerSystem>
    {
        #region Fields

        private readonly List<Timer> _activeTimers = new();
        private readonly List<Timer> _buffer = new();

        #endregion

        #region Life Cycle
        
        protected override void OnUpdate()
        {
            if (_activeTimers.Count == 0) return;

            var deltaTime = Time.deltaTime;
            _buffer.Clear();
            _buffer.AddRange(_activeTimers);

            foreach (var timer in _buffer)
            {
                timer.Tick(deltaTime);
            }
        }

        #endregion

        #region Public Methods

        public void RegisterTimer(Timer timer)
        {
            if (!_activeTimers.Contains(timer))
                _activeTimers.Add(timer);
        }

        public void UnregisterTimer(Timer timer)
        {
            _activeTimers.Remove(timer);
        }

        public void Clear()
        {
            foreach (var timer in _activeTimers)
                timer.Dispose();

            _activeTimers.Clear();
            _buffer.Clear();
        }

        #endregion
    }
}