using System;
using UnityEngine;

namespace SpookyCore.Utilities
{
    public abstract class Timer : IDisposable
    {
        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };
        
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }
        public float Progress => Mathf.Clamp(CurrentTime / _initialTime, 0, 1);
        
        protected float _initialTime;
        private bool _disposed;

        protected Timer(float value)
        {
            _initialTime = value;
        }

        public void Start()
        {
            CurrentTime = _initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimerSystem.Instance.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                TimerSystem.Instance.UnregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }

        public abstract void Tick();
        public abstract bool IsFinished { get; }

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public virtual void Reset() => CurrentTime = _initialTime;

        public virtual void Reset(float newTime)
        {
            _initialTime = newTime;
            Reset();
        }

        ~Timer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                TimerSystem.Instance.UnregisterTimer(this);
            }

            _disposed = true;
        }
    }
}