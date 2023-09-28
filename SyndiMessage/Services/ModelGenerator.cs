using RabbitMQ.Client;
using SyndiMessage.Configs;
using SyndiMessage.Contracts;

namespace SyndiMessage.Services
{
    public class ModelGenerator<TBroker> : IModelGenerator<TBroker> where TBroker : RabbitMqConfig
    {

        public ModelGenerator(IRabbitMqService<TBroker> generator)
        {
            Generator = generator;
        }

        private IRabbitMqService<TBroker> Generator { get; }

        public IModel GenerateModel()
        {
            return Generator.CreateConnection().CreateModel();
        }
    }
}
