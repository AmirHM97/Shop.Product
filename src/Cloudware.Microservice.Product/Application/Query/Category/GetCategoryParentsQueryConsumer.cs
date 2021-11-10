using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MassTransit;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public class GetCategoryParentsQueryConsumer : IConsumer<GetCategoryParentsQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;

        public GetCategoryParentsQueryConsumer(IProductReadDbContext productReadDbContext)
        {
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<GetCategoryParentsQuery> context)
        {
            var categories = await _productReadDbContext.ProductCategoryNormalizedCollection.Find(f => f.TenantId == context.Message.TenantId).ToListAsync();
            var parents=ProductExtensions.GetParents(categories,new(),context.Message.CategoryId);
            parents=parents.Where(w=>w is not null).Reverse().ToList();
            await context.RespondAsync(new GetCategoryParentsQueryResponse(parents));
        }
    }
    public class GetCategoryParentsQueryResponse{
        public List<ProductCategoryNormalizedCollection> ProductCategories { get; set; }

        public GetCategoryParentsQueryResponse(List<ProductCategoryNormalizedCollection> productCategories)
        {
            ProductCategories = productCategories;
        }
    }
   
}