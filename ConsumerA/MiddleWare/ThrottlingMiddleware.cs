using System;
using System.Threading.Tasks;
using MassTransit;

public class ThrottlingMiddleware<T> : IFilter<ConsumeContext<T>>
    where T : class
{    
    private readonly TimeSpan messageInterval;
    private readonly SemaphoreSlim semaphore1 = new SemaphoreSlim(1);

    public ThrottlingMiddleware(int maxMessagesPerSecond)
    {
        this.messageInterval = TimeSpan.FromSeconds(1.0 / maxMessagesPerSecond);
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        await semaphore1.WaitAsync();
        await DelayAsync(messageInterval);    
        semaphore1.Release();
        await next.Send(context);
    }

    private async Task DelayAsync(TimeSpan delay)
    {
        if (delay.TotalMilliseconds > 0)
        {
            await Task.Delay(delay);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("throttle");
    }
}
