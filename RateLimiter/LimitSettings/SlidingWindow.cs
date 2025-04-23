namespace RateLimiter.LimitSettings
{
    public class SlidingWindow : ILimit
    {
        public TimeSpan Window { get; set; }
        public int MaxRepeats { get; set; }
        private DateTime LastCall { get; set; } = DateTime.UtcNow;
        private int Repeats { get; set; } = 0;

        public bool Exceeds(DateTime currentTime)
        {
            return currentTime - LastCall < Window && Repeats >= MaxRepeats;
        }

        public TimeSpan GetRemainingTime(DateTime currentTime)
        {
            return Window - (currentTime - LastCall);
        }

        public void Update(DateTime currentTime)
        {
            Repeats++;
            LastCall = currentTime;
        }

        public void Reset()
        {
            Repeats = 0;
        }
    }
}
