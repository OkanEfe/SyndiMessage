using SyndiMessage.Configs;

namespace SyndiMessage.Contracts
{

    public interface IPublish<TBroker> where TBroker : RabbitMqConfig
    {
        void PublishMessage<TMessage>(TMessage message);
        void PublishMessages<TMessage>(IEnumerable<TMessage> messages);
    }
}
