namespace SpookyCore.Runtime.Systems
{
    public class CountupTimer : Timer
    {
        public CountupTimer(float targetDuration) : base(targetDuration)
        {
            CurrentTime = 0f;
        }

        public override void Tick(float deltaTime)
        {
            if (!IsRunning || IsFinished) return;

            CurrentTime += deltaTime;
            if (CurrentTime >= Duration)
            {
                CurrentTime = Duration;
                MarkFinished();
            }
        }
    }
}