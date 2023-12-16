using RabbitMQ.Client;
using SyndiMessage.Attributes;

namespace Syndicate.Test.Integration.Shared;

[MessageDeclaration("test", "test", "", ExchangeType.Topic)]
public class TestMessageNotNullExchangeName
{
    public string Message { get; set; }
};

[MessageDeclaration("test1","","test1",ExchangeType.Direct)]
public class TestMessageNullExchangeName
{
    public string Message { get; set; }
};
