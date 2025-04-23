using RateLimiter.LimitSettings;

namespace RateLimiter
{
    public class RateLimiter<TArg>
    {
        public required Func<TArg, Task> action { get; set; }
        public required List<ILimit> limits { get; set; }

        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public async Task PerformAsync(TArg argument)
        {
            await semaphore.WaitAsync();

            var now = DateTime.UtcNow;
            var limitsExceed = limits.FindAll(l => l.Exceeds(now)).OrderBy(li => li.Window);

            if (limitsExceed is not null && limitsExceed.Any())
            {
                var waitTime = limitsExceed.FirstOrDefault()!.GetRemainingTime(now);
                await Task.Delay(waitTime);
                limitsExceed.ToList().ForEach(l => l.Reset());
            }

            await action(argument);
            limits.ForEach(l => l.Update(now));

            semaphore.Release();
        }

        public void Perform(TArg argument)
        {
            semaphore.Wait();
            var now = DateTime.UtcNow;
            var limitsExceed = limits.FindAll(l => l.Exceeds(now)).OrderBy(li => li.Window);

            if (limitsExceed is not null && limitsExceed.Any())
            {
                var waitTime = limitsExceed.FirstOrDefault()!.GetRemainingTime(now);
                Thread.Sleep(waitTime);
                limitsExceed.ToList().ForEach(l => l.Reset());
            }

            action(argument).Wait();
            limits.ForEach(l => l.Update(now));

            semaphore.Release();
        }
    }
}
