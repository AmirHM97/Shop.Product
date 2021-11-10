using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Events.Product;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Product
{
    public class DeleteProductCommandConsumer : IConsumer<DeleteProductCommand>, IMediatorConsumerType
    {
        private readonly DbSet<ProductItem> _products;
        private readonly DbSet<StockItem> _stockItem;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public DeleteProductCommandConsumer(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _products = _unitOfWork.Set<ProductItem>();
            _stockItem = _unitOfWork.Set<StockItem>();
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<DeleteProductCommand> context)
        {
            var product = await _products.Where(w => w.Guid.ToString().ToLower() == context.Message.Guid.ToLower() && w.TenantId == context.Message.TenantId).FirstOrDefaultAsync();
            if (product is null)
                throw new AppException(5056, System.Net.HttpStatusCode.BadRequest, "Product Not Found!!!");
            product.IsDeleted = true;
            product.IsActive = false;
            var stocks = await _stockItem.Where(w => w.ProductId == product.Id).ToListAsync();
            stocks = stocks.Select(s => { s.IsDeleted = true; s.IsAvailable = false; return s; }).ToList();
            _products.Update(product);
            _stockItem.UpdateRange(stocks);
            await _unitOfWork.SaveChangesAsync();
            await _mediator.Publish(new ProductDeletedInWriteDbEvent(product.Id));

        }
    }
}