using MassTransit;
using Types;

namespace ConsumerBT.Consumer
{
    public class ConsumerB : IConsumer<Batch<MyMessage>>
    {
        private ILogger<ConsumerB> _logger;

        public ConsumerB(ILogger<ConsumerB> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Batch<MyMessage>> context)
        {
            _logger.Log(LogLevel.Information, "Consumer B: Ya tengo 10 omg");
            
        }
    }
}
