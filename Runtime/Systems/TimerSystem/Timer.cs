using System;

namespace SpookyCore.Runtime.Systems
{
    public abstract class Timer : IDisposable
    {
        public event Action OnTimerStart;
        public event Action OnTimerStop;
        public event Action OnTimerFinish;

        public float CurrentTime { get; protected set; }
        public float Duration { get; protected set; }
        public bool IsRunning { get; private set; }
        public bool IsFinished { get; protected set; }

        protected bool _disposed;

        protected Timer(float duration)
        {
            Duration = duration;
            CurrentTime = duration;
        }

        public void Start()
        {
            if (IsRunning) return;
            IsFinished = false;
            IsRunning = true;
            TimerSystem.Instance.RegisterTimer(this);
            OnTimerStart?.Invoke();
        }

        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;
            TimerSystem.Instance.UnregisterTimer(this);
            OnTimerStop?.Invoke();
        }

        public void Pause() => IsRunning = false;
        public void Resume() => IsRunning = true;

        public void Reset() => CurrentTime = Duration;

        public void SetDuration(float newDuration)
        {
            Duration = newDuration;
            Reset();
        }

        public abstract void Tick(float deltaTime);

        public void MarkFinished()
        {
            IsFinished = true;
            Stop();
            OnTimerFinish?.Invoke();
        }

        public void Dispose()
        {
            if (_disposed) return;
            Stop();
            _disposed = true;
        }
    }
}