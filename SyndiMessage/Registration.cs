using Microsoft.Extensions.DependencyInjection;
using SyndiMessage.Configs;
using SyndiMessage.Contracts;
using SyndiMessage.Services;
using System.Text.Json;

namespace SyndiMessage;

public static class Registration
{

    public static IServiceCollection AddPublisher(this IServiceCollection services)
    {
        services.AddSingleton<JsonSerializerOptions>(opt => new JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        });
        services.AddSingleton(typeof(IPublish<>), typeof(Publish<>));
        return services;
    }

    public static IServiceCollection AddBroker<TBroker>(this IServiceCollection services, TBroker brokerConfigs) where TBroker : RabbitMqConfig
    {
        services.AddSingleton(typeof(TBroker), brokerConfigs);
        services.AddSingleton<IRabbitMqService<TBroker>, RabbitMqService<TBroker>>(opt =>
        {
            return new RabbitMqService<TBroker>(brokerConfigs);
        });

        services.AddSingleton<IModelGenerator<TBroker>, ModelGenerator<TBroker>>();

        return services;
    }

    public static IServiceCollection AddBroker<TBroker>(this IServiceCollection services, Func<IServiceProvider, TBroker> brokerConfiguration) where TBroker : RabbitMqConfig
    {
        services.AddSingleton(typeof(TBroker), brokerConfiguration);
        services.AddSingleton<IRabbitMqService<TBroker>, RabbitMqService<TBroker>>(opt =>
        {
            var brokerConfig = brokerConfiguration.Invoke(opt);
            return new RabbitMqService<TBroker>(brokerConfig);
        });

        services.AddSingleton<IModelGenerator<TBroker>, ModelGenerator<TBroker>>();

        return services;
    }
}
