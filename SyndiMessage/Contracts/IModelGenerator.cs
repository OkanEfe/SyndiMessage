using RabbitMQ.Client;
using SyndiMessage.Configs;

namespace SyndiMessage.Contracts;

public interface IModelGenerator<TBrokerConfiguration> where TBrokerConfiguration : RabbitMqConfig
{
    IModel GenerateModel();
}
