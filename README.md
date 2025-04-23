## RateLimiter Library

A custom `RateLimiter<TArg>` library was created to control execution of threads and asynchronous tasks based on configurable rate limits.

xUnit tests - added.

**RateLimiter.PerformAsync(...)** - for asynchronous task execution.

**RateLimiter.Perform(...)** - thread-based execution, blocking until allowed.

Why two variants? - educational purposes.

### Strategy

The implementation uses the **Sliding Window** algorithm to enforce limits.

### Why Sliding Window?

- **Accurate** — tracks requests in the last N seconds, not fixed blocks.
- **Prevents bursts** — avoids spikes at interval boundaries.
- **Fair** — ensures consistent limits for all users over time.
