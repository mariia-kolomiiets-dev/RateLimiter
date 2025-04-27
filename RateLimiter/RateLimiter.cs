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
            try
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

                limits.ForEach(l => l.Update(now));
            }
            catch (Exception)
            {

            }
            finally
            {
                semaphore.Release();
            }

            await action(argument);
        }

        public void Perform(TArg argument)
        {
            try
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

                limits.ForEach(l => l.Update(now));
            }
            catch (Exception)
            {

            }
            finally
            {
                semaphore.Release();
            }

            action(argument).Wait();
        }
    }
}
