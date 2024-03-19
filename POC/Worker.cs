using MassTransit;
using Types;

namespace Publisher
{
    public class Worker : BackgroundService
    {
        readonly IBus _bus;
        private readonly ILogger<Worker> _logger;
        int counter = 0;
        public Worker(IBus bus, ILogger<Worker> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var counter = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                await _bus.Publish(new MyMessage
                {
                    Body =  $"Hola: {counter}"
                }) ;
                counter++;

                if(counter %10 == 0)
                {
                    _logger.Log(LogLevel.Information, "He publicado 10");
                }
                await Task.Delay(10);
            }
        }
    }
}
