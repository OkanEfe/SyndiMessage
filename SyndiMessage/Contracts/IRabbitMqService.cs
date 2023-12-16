using RabbitMQ.Client;

namespace SyndiMessage.Services;

public interface IRabbitMqService<TBroker>
{
    public IConnection CreateConnection();
}