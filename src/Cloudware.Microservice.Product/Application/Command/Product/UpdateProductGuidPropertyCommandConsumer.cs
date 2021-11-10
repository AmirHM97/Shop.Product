using System;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Command.Product
{
    public class UpdateProductGuidPropertyCommandConsumer : IConsumer<UpdateProductGuidPropertyCommand>, IMediatorConsumerType
    {
        private readonly DbSet<ProductItem> _productItems;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductReadDbContext _productReadDbContext;

        public UpdateProductGuidPropertyCommandConsumer(IUnitOfWork unitOfWork, IProductReadDbContext productReadDbContext)
        {
            _unitOfWork = unitOfWork;
            _productItems = _unitOfWork.Set<ProductItem>();
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<UpdateProductGuidPropertyCommand> context)
        {
            var wProducts = await _productItems.ToListAsync();
            wProducts = wProducts.Select(s => { s.Guid = Guid.NewGuid(); return s;}).ToList();
            _productItems.UpdateRange(wProducts);
            await _unitOfWork.SaveChangesAsync();
            foreach (var item in wProducts)
            {
                var update= Builders<ProductItemReadDbCollection>.Update
                .Set(s=>s.Guid,item.Guid.ToString());
                await _productReadDbContext.ProductItemsDataCollection.UpdateOneAsync(u=>u.ProductId==item.Id,update);
            }
        }
    }
}