using Types;

namespace ConsumerAT.Processor
{
    public interface IMessageProcessor
    {
        Task ProcessMessage(MyMessage message);
    }
}
