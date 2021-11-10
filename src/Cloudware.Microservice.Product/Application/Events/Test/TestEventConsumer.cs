using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Product;
using DnsClient.Internal;
using Elasticsearch.Net.Specification.IndicesApi;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test;

namespace Cloudware.Microservice.Product.Application.Events.Test
{
    public class TestEventConsumer : IConsumer<TestEvent>,IBusConsumerType
    {
        private readonly ILogger<TestEventConsumer> _logger;

        public TestEventConsumer(ILogger<TestEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TestEvent> context)
        {
            _logger.LogInformation("Hello From Product TestConsumer :>");
            await context.RespondAsync(new GetProductForBasketQueryResponse());
        }
    }
}
