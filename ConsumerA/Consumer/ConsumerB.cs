using MassTransit;
using Types;

namespace ConsumerBT.Consumer
{
    public class ConsumerB : IConsumer<MyMessage>
    {
        private ILogger<ConsumerB> _logger;

        public ConsumerB(ILogger<ConsumerB> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MyMessage> context)
        {
            _logger.Log(LogLevel.Information, "ConsumerB " + context.Message.Body);
            Thread.Sleep(10000);
            
        }
    }
}
