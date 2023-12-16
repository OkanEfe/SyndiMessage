using Moq;
using RabbitMQ.Client;
using Syndicate.Test.Integration.Shared;
using SyndiMessage.Configs;
using SyndiMessage.Services;

namespace Syndicate.Test.Integration;

public class BrokerConnectionTest : IClassFixture<RabbitMqService>
{
    private readonly RabbitMqService _rabbitMqService;

    public BrokerConnectionTest(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    [Fact]
    public void Connection_ShouldSuccess_OnLocalBroker()
    {
        IConnection connection = _rabbitMqService.CreateConnection();

        Assert.NotNull(connection);
        Assert.True(connection.IsOpen);
        Assert.Equal("localhost", connection.Endpoint.HostName);

        connection.Close();

        Assert.False(connection.IsOpen);
    }
}