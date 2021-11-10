using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Events;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public class DeleteCategoryCommandConsumer : IConsumer<DeleteCategoryCommand>, IMediatorConsumerType
    {
        private readonly DbSet<ProductCategory> _productCategories;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly IMediator _mediator;
        public DeleteCategoryCommandConsumer(IUnitOfWork unitOfWork, IMediator mediator, IProductReadDbContext productReadDbContext)
        {
            _unitOfWork = unitOfWork;
            _productCategories = _unitOfWork.Set<ProductCategory>();
            _mediator = mediator;
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<DeleteCategoryCommand> context)
        {
            
            var parent = await _productReadDbContext.ProductCategoryNormalizedCollection.Find(f => f.Guid.ToLower() == context.Message.Guid.ToLower() && f.TenantId == context.Message.TenantId).FirstOrDefaultAsync();
            var children = await _productReadDbContext.ProductCategoryNormalizedCollection.Find(f => f.ParentId == parent.CategoryId && f.TenantId == context.Message.TenantId).ToListAsync();
            children.Add(parent);
            var childrenIds = children.Select(s => s.CategoryId).ToList();
            var productsAvailable = await _productReadDbContext.ProductItemsDataCollection.Find(f => childrenIds.Contains(f.CategoryId) && !f.IsDeleted.Value).AnyAsync();

            if (productsAvailable)
                throw new AppException(5056, System.Net.HttpStatusCode.BadRequest, "category has products");

            var categories = await _productCategories.Where(w => children.Select(s => s.CategoryId).Contains(w.Id) && w.TenantId == context.Message.TenantId).ToListAsync();
            categories = categories.Select(s => { s.IsDeleted = true; return s; }).ToList();
            _productCategories.UpdateRange(categories);
            await _unitOfWork.SaveChangesAsync();

            await _mediator.Publish(new UpdateReadDbCategoryCommand(context.Message.TenantId));

        }
    }
}