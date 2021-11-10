using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events.Test
{
    public class faultTest:IRequestType

    {
        
    }
    public class faultTestresponse
    {

    }
    public class faultTestConsumer : IConsumer<faultTest>,IMediatorConsumerType,IBusConsumerType


    {
        public async Task Consume(ConsumeContext<faultTest> context)
        {
                
            throw new NotImplementedException();
        }
    }
    public class faultTestFaultedConsumer : IConsumer<Fault<faultTest>>,IBusConsumerType

    {
        public async Task Consume(ConsumeContext<Fault<faultTest>> context)
        {
            var a = 015;
            throw new NotImplementedException();
        }
    }
}
