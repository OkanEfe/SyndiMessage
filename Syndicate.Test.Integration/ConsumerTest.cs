
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Syndicate.Test.Integration.Shared;
using SyndiMessage.Services;

namespace Syndicate.Test.Integration;
public class ConsumerTest : IClassFixture<RabbitMqService>
{
    private RabbitMqService _rabbitMqService;

    public ConsumerTest(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }
    [Fact]
    public void Consume_ShouldSuccess_ExistQueue()
    {
        ModelGenerator<TestConfig> modelGenerator = new(_rabbitMqService);
        TestConsumer consumer = new(modelGenerator, null);
        
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();

        var model = _rabbitMqService.CreateConnection().CreateModel();

        Assert.True(model.ConsumerCount("test")>0);
    }
}
