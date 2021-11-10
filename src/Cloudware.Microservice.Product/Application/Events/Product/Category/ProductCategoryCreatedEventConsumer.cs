using Cloudware.Microservice.Product.Application.Command.Category;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events.Product.Category
{
    public class ProductCategoryCreatedEventConsumer : IConsumer<ProductCategoryCreatedEvent>, IMediatorConsumerType
    {
        private readonly IMediator _mediator;

        public ProductCategoryCreatedEventConsumer( IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ProductCategoryCreatedEvent> context)
        {
            await _mediator.Publish(new UpdateReadDbCategoryCommand(context.Message.ClientId));
        }
    }
}
