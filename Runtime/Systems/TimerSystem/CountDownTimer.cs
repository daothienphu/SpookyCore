namespace SpookyCore.Runtime.Systems
{
    public class CountDownTimer : Timer
    {
        public CountDownTimer(float duration) : base(duration) { }

        public override void Tick(float deltaTime)
        {
            if (!IsRunning || IsFinished) return;

            CurrentTime -= deltaTime;
            if (CurrentTime <= 0f)
            {
                CurrentTime = 0f;
                MarkFinished();
            }
        }
    }
}