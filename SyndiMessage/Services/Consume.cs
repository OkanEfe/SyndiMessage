﻿using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SyndiMessage.Attributes;
using SyndiMessage.Configs;
using SyndiMessage.Contracts;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace SyndiMessage.Services;

public abstract class Consumer<TMessage, TBroker> where TMessage : class, new()
    where TBroker : RabbitMqConfig
{
    private int retryCount = 3;

    public int RetryCount
    {
        get
        {
            if (retryCount == 0)
                retryCount = 3;
            return retryCount;
        }
        set
        {
            if (value <= 0)
                retryCount = 3;
            else
                retryCount = value;
        }
    }

    public Consumer(IModelGenerator<TBroker> model, ILogger<Consumer<TMessage, TBroker>> logger)
    {
        Model = model.GenerateModel();
        (QueueName, ExchangeName, RoutingKey, ExchangeType, PrefetchCount) = GetMessageDeclaration();
        (AutoAck, AutoDelete, Durable, Exclusive) = GetBindingDeclaration();

        if (!string.IsNullOrEmpty(ExchangeName))
        {
            Model.ExchangeDeclare(ExchangeName, ExchangeType);
            Model.QueueDeclare(QueueName, Durable, Exclusive, AutoDelete, null);
            Model.QueueBind(QueueName, ExchangeName, RoutingKey);
        }
        else
        {
            Model.QueueDeclare(QueueName, Durable, Exclusive, AutoDelete, null);
        }
        Model.BasicQos(0, PrefetchCount, false);

        _consumer = new AsyncEventingBasicConsumer(Model);
        _consumer.Received += HandleMessage;
        Model.BasicConsume(QueueName, AutoAck, _consumer);
        Logger = logger;
    }

    protected TMessage? Message { get; private set; }
    private IModel Model { get; }
    private string ExchangeName { get; }
    private string QueueName { get; }
    private string RoutingKey { get; }
    private string ExchangeType { get; }
    private ushort PrefetchCount { get; }
    private bool Durable { get; }
    private bool Exclusive { get; }
    private bool AutoDelete { get; }
    private bool AutoAck { get; }

    private AsyncEventingBasicConsumer _consumer { get; }
    public ILogger<Services.Consumer<TMessage, TBroker>> Logger { get; }

    private async Task HandleMessage(object? sender, BasicDeliverEventArgs args)
    {
        var message = Encoding.UTF8.GetString(args.Body.ToArray());
        try
        {
            Message = JsonSerializer.Deserialize(message, typeof(TMessage)) as TMessage;
            await Handle();
            Model.BasicAck(args.DeliveryTag, false);
            Logger.LogInformation($@"{GetType().Name} successfully processed message. Message :{message}");
        }
        catch (Exception e)
        {
            Logger.LogError(e, message);
            if (RetryCount > 1)
            {
                int messageRetryCount = 0;
                var headers = args.BasicProperties.Headers;

                if (headers is not null && headers.ContainsKey("retryCount"))
                    messageRetryCount = (int)headers["retryCount"];

                if (messageRetryCount >= RetryCount)
                    Model.BasicAck(args.DeliveryTag, multiple: false);
                else
                {
                    messageRetryCount += 1;
                    var props = Model.CreateBasicProperties();
                    props.Headers ??= new Dictionary<string, object>();
                    props.Headers["retryCount"] = messageRetryCount;
                    Model.BasicPublish(args.Exchange, args.RoutingKey, basicProperties: props, body: args.Body);
                    Model.BasicReject(args.DeliveryTag, requeue: false);
                }
            }
            else
                Model.BasicReject(args.DeliveryTag, requeue: false);
        }
    }

    private (string, string, string, string, ushort) GetMessageDeclaration()
    {
        var attribute = typeof(TMessage).GetCustomAttribute<MessageDeclarationAttribute>() ??
                        throw new Exception("Message description is null.");
        ;
        return (attribute.QueueName, attribute.ExchangeName, attribute.RoutingKey,
            attribute.ExchangeType ?? "topic", attribute.PrefetchCount);
    }

    private (bool, bool, bool, bool) GetBindingDeclaration()
    {
        var attribute = typeof(TMessage).GetCustomAttribute<BindingDeclarationAttribute>();

        if (attribute == null)
            return (false, false, false, false);

        return (attribute.AutoAck, attribute.AutoDelete, attribute.Durable,
            attribute.Exclusive);
    }

    public abstract Task Handle();
}
