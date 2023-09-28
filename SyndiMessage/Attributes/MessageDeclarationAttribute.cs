namespace SyndiMessage.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageDeclarationAttribute : Attribute
    {
        public string QueueName { get; init; }
        public string ExchangeName { get; init; }
        public string RoutingKey { get; init; }
        public string ExchangeType { get; set; }
        public ushort PrefetchCount { get; set; }

        public MessageDeclarationAttribute(string queueName = "", string exchangeName = "", string routingKey = "", string exchangeType = "topic", ushort prefetchCount = 0)
        {
            QueueName = queueName;
            ExchangeName = exchangeName;
            RoutingKey = routingKey;
            ExchangeType = exchangeType;
            PrefetchCount = prefetchCount;
        }
    }
}
