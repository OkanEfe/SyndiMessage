using RabbitMQ.Client;
using SyndiMessage.Attributes;
using SyndiMessage.Configs;
using SyndiMessage.Contracts;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SyndiMessage.Services
{
    public class Publish<TBroker> : IPublish<TBroker> where TBroker : RabbitMqConfig
    {

        public IConnection Connection { get; set; }
        private string? QueueName { get; set; }
        private string? ExchangeName { get; set; }
        private string? RoutingKey { get; set; }
        private string ExchangeType { get; set; }
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

        public void PublishMessage<TMessage>(TMessage message)
        {
            using var channel = Connection.CreateModel();

            (QueueName, ExchangeName, RoutingKey, ExchangeType) = GetMessageDeclaration(typeof(TMessage));

            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType, false);
            channel.QueueDeclare(QueueName, false, false, false, null);
            channel.QueueBind(QueueName, ExchangeName, RoutingKey);

            var queueMessage = JsonSerializer.Serialize(message, _jsonOptions);
            var body = Encoding.UTF8.GetBytes(queueMessage);
            channel.BasicPublish(ExchangeName, RoutingKey, null, body);
        }

        public void PublishMessages<TMessage>(IEnumerable<TMessage> messages)
        {
            using var channel = Connection.CreateModel();

            (QueueName, ExchangeName, RoutingKey, ExchangeType) = GetMessageDeclaration(typeof(TMessage));

            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType, false);
            channel.QueueDeclare(QueueName, false, false, false, null);
            channel.QueueBind(QueueName, ExchangeName, RoutingKey);

            foreach (var item in messages)
            {
                var queueMessage = JsonSerializer.Serialize(item, _jsonOptions);
                var body = Encoding.UTF8.GetBytes(queueMessage);

                channel.BasicPublish(ExchangeName, RoutingKey, null, body);
            }
        }
    }
}
