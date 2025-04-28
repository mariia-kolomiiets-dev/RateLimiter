namespace RateLimiter.LimitSettings
{
    public class SlidingWindow : ILimit
    {
        public TimeSpan Window { get; set; }
        public int MaxRepeats { get; set; }
        private readonly Queue<DateTime> CallHistory = new Queue<DateTime>();

        public bool Exceeds(DateTime currentTime)
        {
            return CallHistory.Count > 0 &&
                    currentTime - CallHistory.Peek() < Window &&
                    CallHistory.Count >= MaxRepeats;
        }

        public TimeSpan GetRemainingTime(DateTime currentTime)
        {
            if (CallHistory.Count == 0)
                return TimeSpan.Zero;

            return Window - (currentTime - CallHistory.Peek());
        }

        public void Update(DateTime currentTime)
        {
            while (CallHistory.Count > 0 && CallHistory.Peek() <= currentTime - Window)
                CallHistory.Dequeue();

            CallHistory.Enqueue(currentTime);
        }
    }
}
