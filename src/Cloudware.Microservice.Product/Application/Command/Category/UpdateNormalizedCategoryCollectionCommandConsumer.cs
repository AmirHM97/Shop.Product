using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public class UpdateNormalizedCategoryCollectionCommandConsumer : IConsumer<UpdateNormalizedCategoryCollectionCommand>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _readDbContext;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductCategory> _categories;
        public UpdateNormalizedCategoryCollectionCommandConsumer(IUnitOfWork uow, IProductReadDbContext readDbContext)
        {
            _uow = uow;
            _categories = _uow.Set<ProductCategory>();
            _readDbContext = readDbContext;
        }

        public async Task Consume(ConsumeContext<UpdateNormalizedCategoryCollectionCommand> context)
        {
            var CategoryCollection = _readDbContext.ProductCategoryNormalizedCollection;
            await CategoryCollection.DeleteManyAsync(w => w.TenantId == context.Message.TenantId);
            var productCategoriesReadDb = await _categories.Where(w => w.TenantId == context.Message.TenantId&&!w.IsDeleted).Select(s => new ProductCategoryNormalizedCollection
            {
                CategoryId=s.Id,
                CreatedDate=s.CreatedDate,
                LastUpdatedDate=s.LastUpdatedDate,
                Description=s.Description,
                FormId=s.FormId,
                Icon=s.Icon,
                Name=s.Name,
                Guid=s.Guid.ToString(),
                ParentId=s.ParentId,
                RecordId=s.RecordId,
                TenantId=s.TenantId
                
            }).ToListAsync();
            await CategoryCollection.InsertManyAsync(productCategoriesReadDb);
        }
    }
}