using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public class AddCustomCategoryCommandConsumer : IConsumer<AddCustomCategoryCommand>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductCategory> _categories;

        public AddCustomCategoryCommandConsumer(IProductReadDbContext productReadDbContext, IUnitOfWork uow)
        {
            _productReadDbContext = productReadDbContext;
            _uow = uow;
            _categories = _uow.Set<ProductCategory>();

        }

        public async Task Consume(ConsumeContext<AddCustomCategoryCommand> context)
        {
            var collection = _productReadDbContext.CustomCategoryCollection;

            await collection.DeleteOneAsync(w => w.ClientId == context.Message.TenantId);
            var writeDbCategories = await _categories.Where(w => w.TenantId == context.Message.TenantId && !w.IsDeleted).ToListAsync();
            var productCategoriesReadDb = new List<CustomCategoryCollectionItem>();
            foreach (var category in writeDbCategories.Where(w => !w.ParentId.HasValue).Where(w => !w.IsDeleted).ToList())
            {
                productCategoriesReadDb.Add(new CustomCategoryCollectionItem
                {
                    CreatedDate = category.CreatedDate,
                    LastUpdatedDate = category.LastUpdatedDate,
                    Data = new CustomCategoryData
                    {

                        CategoryId = category.Id,
                        Description = category.Description,
                        Icon = category.Icon,
                        FormId = category.FormId,
                        RecordId = category.RecordId,
                        Guid = category.Guid.ToString(),
                        ParentId = category.ParentId,
                    },
                    Name = category.Name,
                    Children = ProductExtensions.GetCustomCategoriesChildren(writeDbCategories, category.Id)
                });
            }
              var productCategoryCollection = new CustomCategoryCollection
                {
                    ClientId = context.Message.TenantId,
                    ProductCategories = productCategoriesReadDb
                };
                await collection.InsertOneAsync(productCategoryCollection);

        }
    }
}