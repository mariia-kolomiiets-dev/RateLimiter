using RateLimiter;

namespace RateLimiterTests
{
    public class RateLimiterPerformTest
    {
        [Fact]
        public async Task WhenParallelTasksRun_WithMultiplyLimitSettings_ThenTasksRunInOrder()
        {
            ILimit limitSetting1 = new SlidingWindow { MaxRepeats = 100, Window = TimeSpan.FromMinutes(1) };
            ILimit limitSetting2 = new SlidingWindow { MaxRepeats = 3, Window = TimeSpan.FromSeconds(4) };
            ILimit limitSetting3 = new SlidingWindow { MaxRepeats = 1, Window = TimeSpan.FromMilliseconds(500) };

            Func<string, Task> apiFunc = async (arg) =>
            {
                // Simulate some action
                await Task.Delay(100);
            };

            RateLimiter<string> rateLimiter = new RateLimiter<string> { action = apiFunc, limits = [limitSetting1, limitSetting2, limitSetting3] };
            var tasks = Enumerable.Range(1, 10).Select(i => rateLimiter.PerformAsync(i.ToString())).ToList();

            await Task.WhenAll(tasks);
            // Check if all tasks completed
            Assert.True(tasks.TrueForAll(t => t.IsCompleted));
        }

        [Fact]
        public void WhenThreadsRun_WithMultiplyLimitSettings_ThenThreadsRunInOrder()
        {
            ILimit limitSetting1 = new SlidingWindow { MaxRepeats = 100, Window = TimeSpan.FromMinutes(1) };
            ILimit limitSetting2 = new SlidingWindow { MaxRepeats = 3, Window = TimeSpan.FromSeconds(4) };
            ILimit limitSetting3 = new SlidingWindow { MaxRepeats = 1, Window = TimeSpan.FromMilliseconds(500) };

            Func<string, Task> apiFunc = (arg) =>
            {
                // Simulate some action
                Thread.Sleep(100);
                return Task.FromResult("");
            };

            RateLimiter<string> rateLimiter = new RateLimiter<string> { action = apiFunc, limits = [limitSetting1, limitSetting2, limitSetting3] };
            var threads = Enumerable.Range(1, 10).Select(i => new Thread(() => rateLimiter.Perform(i.ToString()))).ToList();
            threads.ForEach(t => t.Start());

            while (threads.Any(t => t.IsAlive))
            {
                Thread.Sleep(500);
            }

            // Check if all threads stopped
            Assert.True(threads.TrueForAll(t => !t.IsAlive));
        }
    }
}
