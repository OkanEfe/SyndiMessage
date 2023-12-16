using RabbitMQ.Client;
using SyndiMessage.Configs;

namespace SyndiMessage.Services;

internal class RabbitMqService<TBroker> : IRabbitMqService<TBroker> where TBroker : RabbitMqConfig
{

    public RabbitMqService(TBroker rabbitMqConfig)
    {
        RabbitMqConfig = rabbitMqConfig;
    }

    private TBroker RabbitMqConfig { get; set; }

    public IConnection CreateConnection()
    {
        ConnectionFactory connection = new ConnectionFactory()
        {
            UserName = RabbitMqConfig.UserName,
            Password = RabbitMqConfig.Password,
            HostName = RabbitMqConfig.Host,
            Port = RabbitMqConfig.Port,
            DispatchConsumersAsync = true
        };

        if (RabbitMqConfig.SslOption is not null)
            connection.Ssl = RabbitMqConfig.SslOption;

        var channel = connection.CreateConnection();
        return channel;
    }
}