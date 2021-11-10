using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Product;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Utilities.Common.Exceptions;
using System.Net;

namespace Cloudware.Microservice.Product.Application.Query.Product
{
    public class GetProductForBasketQueryConsumer : IConsumer<GetProductForBasketQuery>, IBusConsumerType
    {
        #region comment
        //private readonly ILogger<GetProductForBasketQueryConsumer> _logger;

        //public GetProductForBasketQueryConsumer(ILogger<GetProductForBasketQueryConsumer> logger)
        //{
        //    _logger = logger;
        //}

        //public async Task Consume(ConsumeContext<GetProductForBasketQuery> context)
        //{
        //    _logger.LogInformation("Hello From Product GetProductForBasketQueryConsumer :>");
        //    await context.RespondAsync(new GetProductForBasketQueryResponse());
        //} 
        #endregion
        private readonly ILogger<GetProductForBasketQueryConsumer> _logger;
        private readonly IProductReadDbContext _readDbContext;
        public GetProductForBasketQueryConsumer(ILogger<GetProductForBasketQueryConsumer> logger, IProductReadDbContext readDbContext)
        {
            _logger = logger;
            _readDbContext = readDbContext;
        }
        public async Task Consume(ConsumeContext<GetProductForBasketQuery> context)
        {
                var collection = _readDbContext.ProductItemsDataCollection;

                var ids = context.Message.GetProductForBasketQueryItems
                .Select(s => s.ProductId).ToList();
                var products = await collection.Find(f => ids.Contains(f.ProductId)).ToListAsync();
                var response = new GetProductForBasketQueryResponse
                {
                    ProductInfo = new List<GetProductForBasketQueryResponseItem>()
                };
                foreach (var item in context.Message.GetProductForBasketQueryItems)
                {
                    var productData = products.FirstOrDefault(w => w.ProductId == item.ProductId);
                    //var stockData = products.Where(w => w.ProductId == item.ProductId).Select(s => s.StockItemsDto)
                    //    .Where(w => w.Select(s => s.StockId).Contains(item.StockId)).FirstOrDefault();
                    var stockData = productData.StockItemsDto.FirstOrDefault(w => w.StockId == item.StockId);
                     if (stockData.IsDeleted ||!stockData.IsAvailable ||!productData.IsAvailable)
                    {
                        throw new AppException(5056,HttpStatusCode.BadRequest,"insufficient stock quantity!!!");
                    }
                    if (stockData.Count<=0 ||item.RequestedCount>stockData.Count)
                    {
                        throw new AppException(5056,HttpStatusCode.BadRequest,"insufficient stock quantity!!!");
                    }
                    response.ProductInfo.Add(new GetProductForBasketQueryResponseItem
                    {
                        ProductId = productData.ProductId,
                        SellerName = productData.UserName,
                        SellerAddress = productData.Address,
                        SellerCity = productData.City,
                        SellerCityId = productData.CityId ?? 0,
                        SellerMobile = productData.Mobile,
                        SellerProvince = productData.Province,
                        SellerProvinceId = productData.ProvinceId ?? 0,
                        RequestedCount = item.RequestedCount,
                        PictureUrl = productData.ImageUrl,
                        ProductName = productData.Name,
                        Discount = stockData.Discount,
                        SellerUserId = productData.UserId,
                        CategoryId = productData.CategoryId,
                        CategoryName = productData.CategoryName,
                        ProductCode=productData.Code,
                        StockItemDto = new StockItemDto
                        {
                            Id = stockData.StockId,
                            Count = stockData.Count,
                            Price = stockData.Price,
                            //TODO: fix here
                            StockPropertyItems = stockData.PropItems.Select(s => new StockPropertyItem { PropertyId = s.PropertyId, PropertyType = s.PropertyType }).ToList()
                        },
                        Properties = productData.PropertiesDto.Select(s => new PropertyDto
                        {
                            Name = s.Name,
                            //TODO: Fix here =>add propertyType and propertyViewType
                            PropertyType = s.PropertyType,
                            PropertyViewType = s.PropertyViewType,
                            PropertyItemsDto = s.PropertyItemDtos.Select(ss => new PropertyItemDto
                            {
                                Id = ss.Id,
                                Name = ss.Name,
                                Value = ss.Value
                            }).ToList()
                        }).ToList()
                    });
                }
                await context.RespondAsync(response);
            //var response = products.Where(w => context.Message.GetProductForBasketItems
            //  .Select(s => s.StockId).Contains(w.StockItemsDto.Select(si))
        }
    }
}
