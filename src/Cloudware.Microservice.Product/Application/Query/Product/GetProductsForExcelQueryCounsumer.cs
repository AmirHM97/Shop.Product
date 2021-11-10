using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Product
{
    public class GetProductsForExcelQueryCounsumer : IConsumer<GetProductsForExcelQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _readDbContext;

        public GetProductsForExcelQueryCounsumer(IProductReadDbContext readDbContext)
        {
            _readDbContext = readDbContext;
        }

       
        public async Task Consume(ConsumeContext<GetProductsForExcelQuery> context)
        {
            var collection = _readDbContext.ProductItemsDataCollection;        
            var productItemsListCatalogDto = await collection.Find(f => true).Project(p => new ProductItemsListCatalogDto
                     {
                         Id = p.ProductId,
                         image = p.ImageUrl,
                         Name = p.Name,
                         Discount = p.StockItemsDto.OrderBy(o => o.Price).Select(s => s.Discount).FirstOrDefault(),
                         Price = p.StockItemsDto.OrderBy(o => o.Price).Select(s => s.Price).FirstOrDefault()
                     }).ToListAsync();
            var result = new GetAllProductsQueryResponse(productItemsListCatalogDto);
            await context.RespondAsync(result);
        }

        public class GetAllProductsQueryResponse
        {
            public List<ProductItemsListCatalogDto> ProductItemsListCatalogsDto { get; set; }

            public GetAllProductsQueryResponse(List<ProductItemsListCatalogDto> productItemsListCatalogsDto)
            {
                ProductItemsListCatalogsDto = productItemsListCatalogsDto;
            }
        }
    }
}
