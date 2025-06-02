using System;

namespace SpookyCore.Runtime.Systems
{
    public class LoopingTimer : Timer
    {
        private readonly Action _onLoop;

        public LoopingTimer(float duration, Action onLoop) : base(duration)
        {
            _onLoop = onLoop;
        }

        public override void Tick(float deltaTime)
        {
            if (!IsRunning) return;

            CurrentTime -= deltaTime;
            if (CurrentTime <= 0f)
            {
                _onLoop?.Invoke();
                Reset();
            }
        }
    }
}