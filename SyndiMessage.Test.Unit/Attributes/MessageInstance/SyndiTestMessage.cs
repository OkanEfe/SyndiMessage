using SyndiMessage.Attributes;
using SyndiMessage.Models;

namespace SyndiMessage.Test.Unit.Attributes.MessageInstance
{
    [MessageDeclaration(queueName:"SyndiQueue",exchangeName:"SyndiExchange",routingKey:"SyndiRoute",exchangeType:ExchangeTypes.topic,0)]
    public class SyndiTestMessage
    {
    }
}
