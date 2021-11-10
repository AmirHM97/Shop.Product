using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public class GetCategoryChildrenIdQueryConsumer : IConsumer<GetCategoryChildrenIdQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductCategory> _productCategories;

        public GetCategoryChildrenIdQueryConsumer(IProductReadDbContext productReadDbContext, IUnitOfWork uow)
        {
            _productReadDbContext = productReadDbContext;
            _uow = uow;
            _productCategories = _uow.Set<ProductCategory>();
        }

        public async Task Consume(ConsumeContext<GetCategoryChildrenIdQuery> context)
        {
 
            var collection = _productReadDbContext.ProductCategoriesDataCollection;
            var cIds = new List<long>();
            //  var categories = await collection.Find(f => f.ClientId == context.Message.TenantId).FirstOrDefaultAsync();
            var cats = await _productCategories.Where(w => w.TenantId==context.Message.TenantId).ToListAsync();
            //var data=await collection.Find(f=>context.Message.CategoryIds.Contains(f.ProductCategories))
            foreach (var item in context.Message.CategoryIds)
            {
                cIds.AddRange(ProductExtensions.GetCategoriesChildrenIdsRead(cats, context.Message.TenantId, item));
            }
            cIds.AddRange(context.Message.CategoryIds);
            await context.RespondAsync(new GetCategoryChildrenIdQueryResponse(cIds));


        }
    }

    public class GetCategoryChildrenIdQueryResponse
    {
        public List<long> CategoryIds { get; set; }

        public GetCategoryChildrenIdQueryResponse(List<long> categoryIds)
        {
            CategoryIds = categoryIds;
        }
    }
}