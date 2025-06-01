using System.Collections.Generic;

namespace SpookyCore.Utilities
{
    public class TimerSystem : MonoSingleton<TimerSystem> {
        private readonly List<Timer> _timers = new();
        private readonly List<Timer> _sweep = new();
        
        public void RegisterTimer(Timer timer) => _timers.Add(timer);
        public void UnregisterTimer(Timer timer) => _timers.Remove(timer);

        protected override void OnUpdate()
        {
            if (_timers.Count == 0) return;
            
            _sweep.Clear();
            _sweep.AddRange(_timers);
            foreach (var timer in _sweep) {
                timer.Tick();
            }
        }
        
        public void DisposeTimers() {
            _sweep.Clear();
            _sweep.AddRange(_timers);
            foreach (var timer in _sweep) {
                timer.Dispose();
            }
            
            _timers.Clear();
            _sweep.Clear();
        }
    }
}