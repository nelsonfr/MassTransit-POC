using ConsumerAT.Processor;
using MassTransit;
using Types;

namespace ConsumerAT.Consumer
{
    public class ConsumerA : IConsumer<MyMessage>
    {
        private readonly ILogger<ConsumerA> _logger;
        private readonly IMessageProcessor _processor;
        private static int counter = 0;

        public ConsumerA(ILogger<ConsumerA> logger, IMessageProcessor processor)
        {
            _logger = logger;
            _processor = processor;
        }
        public async Task Consume(ConsumeContext<MyMessage> context)
        {
           await _processor.ProcessMessage(context.Message);
        }
    }
}
