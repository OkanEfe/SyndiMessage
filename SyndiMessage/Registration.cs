using Microsoft.Extensions.DependencyInjection;
using SyndiMessage.Configs;
using SyndiMessage.Contracts;
using SyndiMessage.Services;
using System.Text.Json;

namespace SyndiMessage
{
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

        public static IServiceCollection AddBrokers<TBroker>(this IServiceCollection services, IEnumerable<TBroker> brokers) where TBroker : RabbitMqConfig
        {
            foreach (TBroker broker in brokers)
            {
                services.AddBroker(broker);
            }

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
    }
}
