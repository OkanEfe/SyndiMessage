using SyndiMessage.Attributes;
using SyndiMessage.Test.Unit.Attributes.MessageInstance;
using System.Reflection;

namespace SyndiMessage.Test.Unit.Attributes
{
    public class MessageDeclarationAttributeTest
    {
        [Fact]
        public void MessageType_ShouldHave_MessageDeclaration()
        {
            SyndiTestMessage message = new SyndiTestMessage();

            var attribute = message.GetType().GetCustomAttributes(typeof(MessageDeclarationAttribute), false).Any();

            Assert.True(attribute);
        }

        [Fact]
        public void MessageType_ShouldHave_MessageDeclarationFields()
        {
            SyndiTestMessage message = new SyndiTestMessage();

            var attribute = message.GetType().GetCustomAttribute<MessageDeclarationAttribute>();

            Assert.NotNull(attribute);
            Assert.NotNull(attribute.QueueName);
            Assert.NotEmpty(attribute.QueueName);
            Assert.NotNull(attribute.ExchangeName);
            Assert.NotEmpty(attribute.ExchangeName);
            Assert.NotNull(attribute.ExchangeType);
            Assert.NotEmpty(attribute.ExchangeType);
            Assert.NotNull(attribute.RoutingKey);
            Assert.NotEmpty(attribute.RoutingKey);
            Assert.Equal(0, attribute.PrefetchCount);
        }
    }
}
