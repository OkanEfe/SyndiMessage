using Microsoft.Extensions.Logging;
using SyndiMessage.Contracts;
using SyndiMessage.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syndicate.Test.Integration.Shared;
internal class TestConsumer : Consumer<TestMessageNotNullExchangeName, TestConfig>
{
    public TestConsumer(IModelGenerator<TestConfig> model, ILogger<Consumer<TestMessageNotNullExchangeName, TestConfig>> logger) : base(model, logger)
    {
    }

    public override Task Handle()
    {
        return Task.FromResult("success");
    }
}
