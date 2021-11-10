using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Events.Stock
{
    public class StockEditedEventConsumer : IConsumer<StockEditedEvent>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly DbSet<PropertyCategory> _propertyCategories;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<StockItem> _stockItems;
        private readonly DbSet<Color> _colors;
        private readonly DbSet<Guarantee> _guarantees;
        private readonly DbSet<Size> _sizes;
        private readonly IPropertyService _propertyService;


        public StockEditedEventConsumer(IProductReadDbContext productReadDbContext, IUnitOfWork uow, IPropertyService propertyService)
        {
            _productReadDbContext = productReadDbContext;
            _uow = uow;
            _propertyCategories = _uow.Set<PropertyCategory>();
            _stockItems = _uow.Set<StockItem>();
            _colors = _uow.Set<Color>();
            _guarantees = _uow.Set<Guarantee>();
            _sizes = _uow.Set<Size>();
            _propertyService = propertyService;
        }

        public async Task Consume(ConsumeContext<StockEditedEvent> context)
        {
            var product = await _productReadDbContext.ProductItemsDataCollection.Find(f => f.ProductId == context.Message.ProductId && f.TenantId == context.Message.TenantId).FirstOrDefaultAsync();
            var stocks = await _stockItems.Include(s => s.StockPropertyItems).Where(f => f.ProductId == context.Message.ProductId && !f.IsDeleted).ToListAsync();

            if (product is null)
            {
                throw new AppException(5056, System.Net.HttpStatusCode.BadRequest, "product not found!!!");
            }
            var categoryProperty = await _propertyCategories.Where(w => w.CategoryId == product.CategoryId).ToListAsync();
            if (categoryProperty.Count is not 0)
            {
                var properties = new List<(PropertyType, List<PropertyItemReadDb>)>();

                foreach (PropertyCategory item in categoryProperty)
                {
                    var props = stocks.SelectMany(s => s.StockPropertyItems).Where(w => w.PropertyType == item.PropertyType).Select(s => s.PropertyId).ToList();
                    var propertyItemsReadDb = await _propertyService.GetPropertiesByPropertyType(item, props);
                    properties.Add(new(item.PropertyType, propertyItemsReadDb));
                }

                var propertyReadDb = properties.Select(s => new PropertyReadDb
                {
                    PropertyId = (int)s.Item1,
                    Name = ProductExtensions.GetPropertyName(s.Item1),
                    PropertyType = s.Item1,
                    PropertyViewType = ProductExtensions.GetPropertyViewType(s.Item1),
                    PropertyItemDtos = s.Item2
                }).ToList();
                product.PropertiesDto = propertyReadDb;
                // var stocks = new List<StockItemReadDb>();
                product.StockItemsDto = await _stockItems.Where(w => w.ProductId == context.Message.ProductId && !w.IsDeleted).Select(g => new StockItemReadDb
                {
                    StockId = g.Id,
                    Count = g.Count,
                    Discount = g.Discount,
                    Price = g.Price,
                    MaxCount = g.MaxCount,
                    MinCount = g.MinCount,
                    IsAvailable = g.IsAvailable,

                    PropItems = propertyReadDb.Count == 0 ? new() : g.StockPropertyItems.Select(y => new Model.ReadDbModel.StockPropertyItem
                    {
                        StockPropertyId = y.Id,
                        PropertyId = y.PropertyId,
                        PropertyType = y.PropertyType
                    }).ToList()
                }).ToListAsync();
            }
            else
            {
                product.StockItemsDto = stocks.Select(g =>
                         new StockItemReadDb
                         {
                             StockId = g.Id,
                             Count = g.Count,
                             Discount = g.Discount,
                             Price = g.Price,
                             MaxCount = g.MaxCount,
                             MinCount = g.MinCount,
                             IsAvailable = g.IsAvailable,

                             PropItems = product.PropertiesDto.Count == 0 ? new() : g.StockPropertyItems.Select(y => new Model.ReadDbModel.StockPropertyItem
                             {
                                 StockPropertyId = y.Id,
                                 PropertyId = y.PropertyId,
                                 PropertyType = y.PropertyType
                             }).ToList()
                         }).ToList();
            }
            var filter = Builders<ProductItemReadDbCollection>.Filter.Eq(s => s.ProductId, context.Message.ProductId);
            product.LastUpdatedDate = DateTime.UtcNow;
            await _productReadDbContext.ProductItemsDataCollection.ReplaceOneAsync(filter, product);
        }
    }
}