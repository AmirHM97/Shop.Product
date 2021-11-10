using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Events.Product;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Command.Product
{
    public class DeleteProductListCommandConsumer : IConsumer<DeleteProductListCommand>, IMediatorConsumerType
    {
        private readonly DbSet<ProductItem> _productItems;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public DeleteProductListCommandConsumer(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _productItems = _unitOfWork.Set<ProductItem>();
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<DeleteProductListCommand> context)
        {
            var products = await _productItems.Where(f => f.TenantId == context.Message.TenantId && context.Message.ProductIds.Contains(f.Guid)).ToListAsync();
            products = products.Select(s => { s.IsDeleted = true;s.IsActive=false;return s; }).ToList();
            _productItems.UpdateRange(products);
            await _unitOfWork.SaveChangesAsync();
            foreach (var item in products)
            {
                await _mediator.Publish(new ProductDeletedInWriteDbEvent(item.Id));
            }
        }
    }
}