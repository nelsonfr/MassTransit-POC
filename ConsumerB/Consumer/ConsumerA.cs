using MassTransit;
using Types;

namespace ConsumerAT.Consumer
{
    public class ConsumerA : IConsumer<Batch<MyMessage>>
    {
        private readonly ILogger<ConsumerA> _logger;

        public ConsumerA(ILogger<ConsumerA> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<Batch<MyMessage>> context)
        {
            _logger.Log(LogLevel.Information, "Consumer A: Ya tengo 10 omg, voy a dormir");
        }
    }
}
