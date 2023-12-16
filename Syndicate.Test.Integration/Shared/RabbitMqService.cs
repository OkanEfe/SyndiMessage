using RabbitMQ.Client;
using SyndiMessage.Services;

namespace Syndicate.Test.Integration.Shared;
public class RabbitMqService : IRabbitMqService<TestConfig>
{
    public IConnection CreateConnection()
    {
        ConnectionFactory connection = new ConnectionFactory()
        {
            UserName = "integrationtest",
            Password = "integrationtest",
            HostName = "localhost",
            DispatchConsumersAsync = true
        };

        var channel = connection.CreateConnection();
        return channel;
    }
}
