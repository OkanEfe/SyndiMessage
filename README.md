![N|Solid](https://raw.githubusercontent.com/OkanEfe/SyndiMessage/dev/syndicate-logo.png)

# SyndiMessage

SyndiMessage was developed for easy usage of RabbitMq.Client main library of RabbitMq. 
Using this project, you will enjoy publishing and consuming messages.

## Usage

### Adding Broker
To use this library, the first thing you should do is inject the broker configuration that is beign used. However, **don't forget that** the class being injected must inherit from the RabbitMqConfig class.

After that, you must meet this configuration using the following method :

```cs

serviceCollection.AddBroker(broker);

```

**AddBroker** method is used to configure brokers in the library.

If you need to use multiple brokers, you can use this extension method as many times as you want to configure each broker.


### Publishing Message

"Now that I know you're excited about publishing messages, there's just one last step remaining.

```cs

serviceCollection.AddPublish();

```

After adding the Publisher specification to your application, you should define your message type.

```cs

[MessageDeclaration(queueName:"SyndiQueue",exchangeName:"SyndiExchange",routingKey:"",exchangeType:ExchangeTypes.topic,prefetchCount:0)]
public class SyndiMessage
{
    public string Hello { get; set; }
}

```


For example ,in the above, we added new class named SyndiMessage and use **MessageDeclaration** Attribute to specify information that it will be going to.


Now you can publish your message to any broker you want.

To publish a message, you should inject IPublish instance and use the **PublishMessage** or **PublishMessages** methods.

We wrote a console app as an example of publishing using a provider, but you can inject an IPublish instance into constructors, methods, etc.

```cs
using Microsoft.Extensions.DependencyInjection;
using PublishTest;
using SyndiMessage;
using SyndiMessage.Contracts;

ServiceCollection serviceCollection = new ServiceCollection();

SyndiBroker broker = new SyndiBroker("localhost",5672,"syndi","syndi");

SyndiMessageInstance syndiMessageInstance = new SyndiMessageInstance(AppDomain.CurrentDomain.FriendlyName);

serviceCollection.AddBroker(broker).AddPublisher();

var provider = serviceCollection.BuildServiceProvider();

var publisher = provider.GetRequiredService<IPublish<SyndiBroker>>();

publisher.PublishMessage(syndiMessageInstance);

```

That is all.


### Consuming messages

As you saw message publishing is quite easy. Consumer might been seen more complex than publishing but don't be afraid of. We are all here to assist you :).


As you did at publishing message, you need a message type that specifies message information.

After that, you will need a consumer class that inherits Consumer<TMessage,TBroker>. For example :

```cs

public class SyndiConsumer : Consumer<SyndiMessageInstance, SyndiBroker>
    {
        public SyndiConsumer(IModelGenerator<SyndiBroker> model) : base(model)
        {
        }

        public override async Task Handle()
        {
            await Task.FromResult(Message.FirstMessage);
        }
    }

```

**Consumer<TMessage,TBroker>** base class constructor receives an instance of IModelGenerator<SyndiBroker> instance as a parameter, which you have aldready injected it using the **AddBroker** method.

If you want to add your services, etc., you can inject them using the constructor.

Now you process your message in Handle method using Message property. Message property type is TBroker that you choose already. You don't need to Deserialize the consumed message.

Now you need an instance of your consumer. If you are writing a console application and not using IoC for your services, you must create instances of your services and consumer.

In our opinion, the best practice for creating a Consumer application is to build a worker application. In a worker application, you can use the Reflection library to instantiate your consumers.

For example : 

```cs
var consumers = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("xxx.Consumer")).First().GetTypes().Where(x=>x.BaseType!.IsGenericType && x.BaseType!.GetGenericTypeDefinition()==typeof(Consumer<,>) ).ToList();
var ctorInfos = consumers.Select(x =>
{
    return x.GetConstructors().Single();
});
object current = null;
foreach (ConstructorInfo info in ctorInfos)
{
    object[] parameters = new object[info.GetParameters().Length];
    for (int a = 0; a < info.GetParameters().Length; a++)
    {
        parameters[a] = _serviceProvider.GetService(info.GetParameters()[a].ParameterType);
    
    current = info.Invoke(parameters);
}

```


### Logging

SyndiMessage uses the ILogger interface. If you wish to use another logging library, you should configure it in accordance with the ILogger.

Any exception that occurs is handled in the Consumer base class. It is logged using Log.LogError().
In the Publish interface, there is no any exception handler, so you will need to handle errors yourself.

### Retry message

If an exception occurs in Consumer, the consumed message is not lost. The consumer base class has RetryCount property for sake of retry message to queue.
The default value of this property is 3, but you can adjust it to any value greater than 0. If you don't want to retry the message, you can set it to 1.

For example:

```cs

public class SyndiConsumer : Consumer<SyndiMessageInstance, SyndiBroker>
{
    public SyndiConsumer(IModelGenerator<SyndiBroker> model) : base(model)
    {
        RetryCount = 1;
    }
}

```
