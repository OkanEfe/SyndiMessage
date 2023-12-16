using RabbitMQ.Client;
using SyndiMessage.Attributes;
using SyndiMessage.Configs;
using SyndiMessage.Contracts;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SyndiMessage.Services;

public class Publish<TBroker> : IPublish<TBroker> where TBroker : RabbitMqConfig
{

    public IConnection Connection { get; set; }
    private string QueueName { get; set; }
    private string ExchangeName { get; set; }
    private string RoutingKey { get; set; }
    private string ExchangeType { get; set; }
    private bool Durable { get; set; }
    private bool Exclusive { get; set; }
    private bool AutoDelete { get; set; }
    private bool AutoAck { get; set; }
    private readonly JsonSerializerOptions _jsonOptions;

    public Publish(IRabbitMqService<TBroker> rabbitMqService, JsonSerializerOptions jsonOptions)
    {
        Connection = rabbitMqService.CreateConnection();
        _jsonOptions = jsonOptions;
    }

    private (string, string, string, string) GetMessageDeclaration(Type messageType)
    {
        var attribute = messageType.GetCustomAttribute<MessageDeclarationAttribute>() ??
                        throw new Exception("Message description is null."); ;
        return (attribute.QueueName, attribute.ExchangeName, attribute.RoutingKey, attribute.ExchangeType ?? "topic");
    }
    private (bool, bool, bool, bool) GetBindingDeclaration(Type messageType)
    {
        var attribute = messageType.GetCustomAttribute<BindingDeclarationAttribute>();

        if (attribute == null)
            return (false, false, false, false);

        return (attribute.AutoAck, attribute.AutoDelete, attribute.Durable,
            attribute.Exclusive);
    }
    public void PublishMessage<TMessage>(TMessage message)
    {
        using var channel = Connection.CreateModel();

        (QueueName, ExchangeName, RoutingKey, ExchangeType) = GetMessageDeclaration(typeof(TMessage));
        (AutoAck, AutoDelete, Durable, Exclusive) = GetBindingDeclaration(typeof(TMessage));

        if (!string.IsNullOrEmpty(ExchangeName))
        {
            channel.ExchangeDeclare(ExchangeName, ExchangeType);
            channel.QueueDeclare(QueueName, Durable, Exclusive, AutoDelete, null);
            channel.QueueBind(QueueName, ExchangeName, RoutingKey);
        }
        else {
            channel.QueueDeclare(QueueName, Durable, Exclusive, AutoDelete, null);
        }

        var queueMessage = JsonSerializer.Serialize(message, _jsonOptions);
        var body = Encoding.UTF8.GetBytes(queueMessage);
        channel.BasicPublish(ExchangeName, RoutingKey, null, body);
    }

    public void PublishMessages<TMessage>(IEnumerable<TMessage> messages)
    {
        using var channel = Connection.CreateModel();

        (QueueName, ExchangeName, RoutingKey, ExchangeType) = GetMessageDeclaration(typeof(TMessage));
        (AutoAck, AutoDelete, Durable, Exclusive) = GetBindingDeclaration(typeof(TMessage));

        if (!string.IsNullOrEmpty(ExchangeName))
        {
            channel.ExchangeDeclare(ExchangeName, ExchangeType);
            channel.QueueDeclare(QueueName, Durable, Exclusive, AutoDelete, null);
            channel.QueueBind(QueueName, ExchangeName, RoutingKey);
        }
        else
        {
            channel.QueueDeclare(QueueName, Durable, Exclusive, AutoDelete, null);
        }

        foreach (var item in messages)
        {
            var queueMessage = JsonSerializer.Serialize(item, _jsonOptions);
            var body = Encoding.UTF8.GetBytes(queueMessage);

            channel.BasicPublish(ExchangeName, RoutingKey, null, body);
        }
    }
}
