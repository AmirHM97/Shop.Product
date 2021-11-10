using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Command.Test
{
    public class TestEditProductCategoryCommandConsumer : IConsumer<TestEditProductCategoryCommand>, IMediatorConsumerType
    {
        private readonly DbSet<ProductItem> _products;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductReadDbContext _productReadDbContext;

        public TestEditProductCategoryCommandConsumer(IUnitOfWork unitOfWork, IProductReadDbContext productReadDbContext)
        {
            _unitOfWork = unitOfWork;
            _products = _unitOfWork.Set<ProductItem>();
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<TestEditProductCategoryCommand> context)
        {
            var products = await _products.Where(w => context.Message.Ids.Contains(w.Id)).ToListAsync();
            products = products.Select(s => { s.CategoryId = context.Message.CategoryId; return s; }).ToList();
            _products.UpdateRange(products);
            await _unitOfWork.SaveChangesAsync();
            var category=await _productReadDbContext.ProductCategoryNormalizedCollection.Find(f=>f.CategoryId==context.Message.CategoryId).FirstOrDefaultAsync();
            foreach (var item in products)
            {
                var up=Builders<ProductItemReadDbCollection>.Update.Set(s=>s.CategoryId,context.Message.CategoryId).Set(s=>s.CategoryName,category.Name);
                await _productReadDbContext.ProductItemsDataCollection.UpdateOneAsync(w=>w.ProductId==item.Id,up);
            }

        }
    }
}