using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Command.Product;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Events.Product.Category
{
    public class CategoryDeletedEventConsumer : IConsumer<CategoryDeletedEvent>, IMediatorConsumerType
    {
        private readonly IMediator _mediator;
        private readonly IProductReadDbContext _productReadDbContext;

        public CategoryDeletedEventConsumer(IMediator mediator, IProductReadDbContext productReadDbContext)
        {
            _mediator = mediator;
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<CategoryDeletedEvent> context)
        {
            // var productIds = await _productReadDbContext.ProductItemsDataCollection.Find(f => context.Message.CategoryIds.Contains(f.CategoryId) && f.TenantId == context.Message.TenantId).Project(p => p.ProductId).ToListAsync();

            // await _mediator.Publish(new DeleteProductListCommand(productIds, context.Message.TenantId));
        }
    }
}