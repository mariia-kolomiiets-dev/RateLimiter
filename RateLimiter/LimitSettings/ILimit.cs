namespace RateLimiter.LimitSettings
{
    public interface ILimit
    {
        public TimeSpan Window { get; set; }
        public bool Exceeds(DateTime currentTime);
        public TimeSpan GetRemainingTime(DateTime currentTime);
        public void Update(DateTime currentTime);
    }
}
