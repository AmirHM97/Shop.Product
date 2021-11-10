using Cloudware.Microservice.Product.Application.Command.Category;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public class UpdateReadDbCategoryCommandConsumer : IConsumer<UpdateReadDbCategoryCommand>, IMediatorConsumerType
    {
          private readonly IProductReadDbContext _readDbContext;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductCategory> _categories;
        private readonly IMediator _mediator;

        public UpdateReadDbCategoryCommandConsumer(IUnitOfWork uow, IProductReadDbContext readDbContext, IMediator mediator)
        {
            _uow = uow;
            _categories = _uow.Set<ProductCategory>();
            _readDbContext = readDbContext;
            _mediator = mediator;
        }
        public  async Task Consume(ConsumeContext<UpdateReadDbCategoryCommand> context)
        {
           var writeDbCategories = await _categories.Where(w => w.TenantId == context.Message.TenantId&&!w.IsDeleted).ToListAsync();
            var CategoryCollection = _readDbContext.ProductCategoriesDataCollection;
            var categories = await CategoryCollection.Find(w => w.ClientId == context.Message.TenantId).FirstOrDefaultAsync();
            var productCategoriesReadDb = new List<ProductCategoryReadDb>();
            foreach (var category in writeDbCategories.Where(w => !w.ParentId.HasValue && !w.IsDeleted).ToList())
            {
                productCategoriesReadDb.Add(new ProductCategoryReadDb
                {
                    CreatedDate = category.CreatedDate,
                    LastUpdatedDate = category.LastUpdatedDate,
                    CategoryId = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Icon = category.Icon,
                    FormId = category.FormId,
                    RecordId = category.RecordId,
                    Guid=category.Guid.ToString(),
                    ParentId = category.ParentId,
                    Children = ProductExtensions.GetCategoriesChildren(writeDbCategories, category.Id)
                });
            }
            if (categories == null)
            {
                //create New
                var productCategoryCollection = new ProductCategoryReadDbCollection
                {
                    ClientId = context.Message.TenantId,
                    ProductCategories = productCategoriesReadDb
                };
                await CategoryCollection.InsertOneAsync(productCategoryCollection);
            }
            else
            {
                //replace
                var filter = Builders<ProductCategoryReadDbCollection>.Filter.Eq(e => e.ClientId, context.Message.TenantId);
                await CategoryCollection.ReplaceOneAsync(filter, new ProductCategoryReadDbCollection
                {
                    ClientId = context.Message.TenantId,
                    ProductCategories = productCategoriesReadDb
                });
            }
             await _mediator.Publish(new UpdateNormalizedCategoryCollectionCommand(context.Message.TenantId));
            await _mediator.Publish(new AddCustomCategoryCommand(context.Message.TenantId));
        }
    }
}