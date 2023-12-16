using Syndicate.Test.Integration.Shared;
using SyndiMessage.Attributes;
using SyndiMessage.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Syndicate.Test.Integration;

public class PublishTest : IClassFixture<RabbitMqService>
{
    private RabbitMqService _rabbitMqService;

    public PublishTest(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    [Fact]
    public void PublishMessage_ShouldSuccess_ExchangeNotNull()
    {
        Publish<TestConfig> publisher = new(_rabbitMqService, null);

        publisher.PublishMessage<TestMessageNotNullExchangeName>(new TestMessageNotNullExchangeName() { Message="test"});

        var model = _rabbitMqService.CreateConnection().CreateModel();

        var messageDeclaration = typeof(TestMessageNotNullExchangeName).GetCustomAttribute<MessageDeclarationAttribute>();

        Assert.True(model.MessageCount(messageDeclaration.QueueName) >= 1);
        
    }

    [Fact]
    public void PublishMessage_ShouldSuccess_ExchangeNull()
    {
        Publish<TestConfig> publisher = new(_rabbitMqService, null);

        publisher.PublishMessage<TestMessageNullExchangeName>(new TestMessageNullExchangeName() { Message = "test" });

        var model = _rabbitMqService.CreateConnection().CreateModel();

        var messageDeclaration = typeof(TestMessageNullExchangeName).GetCustomAttribute<MessageDeclarationAttribute>();

        Assert.True(model.MessageCount(messageDeclaration.QueueName) >= 1);

    }
}
