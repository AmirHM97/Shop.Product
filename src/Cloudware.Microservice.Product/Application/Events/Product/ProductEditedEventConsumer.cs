using Cloudware.Microservice.Product.Application.Events.Product;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.People;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events
{
    public class ProductEditedEventConsumer : IConsumer<ProductEditedEvent>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _readContext;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductCategory> _categories;
        private readonly DbSet<ProductBrand> _brands;
        private readonly DbSet<ProductItem> _products;
        private readonly DbSet<StockItem> _stockItems;
        private readonly DbSet<Color> _colors;
        private readonly DbSet<Guarantee> _guarantees;
        private readonly DbSet<Size> _sizes;
        private readonly ILogger<ProductEditedEventConsumer> _logger;
        private readonly DbSet<PropertyCategory> _propertyCategories;

        private readonly IPublishEndpoint _publishEndpoint;
        public ProductEditedEventConsumer(IUnitOfWork uow, ILogger<ProductEditedEventConsumer> logger, IPublishEndpoint publishEndpoint, IProductReadDbContext readContext)
        {
            _uow = uow;
            _categories = _uow.Set<ProductCategory>();
            _brands = _uow.Set<ProductBrand>();
            _stockItems = _uow.Set<StockItem>();
            _products = _uow.Set<ProductItem>();
            _propertyCategories = _uow.Set<PropertyCategory>();
            _colors = _uow.Set<Color>();
            _guarantees = _uow.Set<Guarantee>();
            _sizes = _uow.Set<Size>();
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _readContext = readContext;
        }

        public async Task Consume(ConsumeContext<ProductEditedEvent> context)
        {
            var stocks = await _stockItems.Include(s => s.StockPropertyItems).Where(f => f.ProductId == context.Message.ProductId && !f.IsDeleted).ToListAsync();
            var collection = _readContext.ProductItemsDataCollection;
            var filter = Builders<ProductItemReadDbCollection>.Filter.Eq(s => s.ProductId, context.Message.ProductId);
            var product = await collection.Find(f => f.ProductId == context.Message.ProductId).FirstOrDefaultAsync();

            if (product.SupplierId !=context.Message.ProductItem.SupplierId)
            {
                var getPeopleByIdIntegrationEvent = new GetPeopleByIdIntegrationEvent(context.Message.ProductItem.SupplierId);
                await _publishEndpoint.Publish(getPeopleByIdIntegrationEvent, x => x.CorrelationId = context.Message.ProductItem.Guid);
            }
            //TODO fix here
            product.BrandId = context.Message.EditProductCommand.BrandId ?? 1;
            product.IsAvailable = context.Message.EditProductCommand.IsAvailable;
            product.BrandName = await _brands.Where(w => w.Id == product.BrandId).Select(s => s.Name).FirstOrDefaultAsync();
            //TODO fix here
            product.Description = context.Message.EditProductCommand.Description;
            product.CategoryId = context.Message.EditProductCommand.CategoryId ?? 1;
            product.CategoryName = await _categories.Where(w => w.Id == product.CategoryId).Select(s => s.Name).FirstOrDefaultAsync();
            product.Code = context.Message.EditProductCommand.Code;
            product.LastUpdatedDate = DateTime.UtcNow;
            product.Story=context.Message.EditProductCommand.Story;
            product.ImageUrl = context.Message.EditProductCommand.Image;
            product.ImageUrls = context.Message.EditProductCommand.ImageUrls.ToList();
            product.MaxPrice = context.Message.EditProductCommand.StocksItems.OrderByDescending(o => o.Price).Select(s => s.Price).FirstOrDefault();
            product.MinPrice = context.Message.EditProductCommand.StocksItems.OrderBy(o => o.Price).Select(s => s.Price).FirstOrDefault();
            product.Name = context.Message.EditProductCommand.Name;
            product.SupplierId = context.Message.ProductItem.SupplierId;
            
            if (context.Message.EditProductCommand.TechnicalPropertiesDto.Count != 0 && context.Message.EditProductCommand.TechnicalPropertiesDto is not null)
            {
                product.TechnicalPropertiesReadDb = context.Message.EditProductCommand.TechnicalPropertiesDto.Where(w => w.Name is not null || w.Value is not null).Select(tp => new TechnicalPropertyReadDb { Name = tp.Name, Value = tp.Value }).ToList();
            }
            if (context.Message.EditProductCommand.StocksItems.Count != 0)
            {
                var categoryProperty = await _propertyCategories.Where(w => w.CategoryId == product.CategoryId).ToListAsync();
                if (categoryProperty.Count is not 0)
                {
                    var properties = new List<(PropertyType, List<PropertyItemReadDb>)>();

                    foreach (PropertyCategory item in categoryProperty)
                    {
                        var props = stocks.SelectMany(s => s.StockPropertyItems).Where(w => w.PropertyType == item.PropertyType).Select(s => s.PropertyId).ToList();
                        var propertyItemsReadDb = await GetPropertiesByPropertyType(item, props);
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
                    // foreach (var (item, newStockItem) in from newStockItem in stocks
                    //                                      let item = newStockItem.Id == null ? product.StockItemsDto.FirstOrDefault() : product.StockItemsDto.FirstOrDefault(f => f.StockId == Int64.Parse(newStockItem.Id))
                    //                                      select (item, newStockItem))
                    // {
                    //     if (item == null)
                    //     {
                    //         var newst = new StockItemReadDb
                    //         {
                    //             Count=newStockItem.Count,
                    //             Discount=newStockItem.Discount,
                    //             IsAvailable=newStockItem.IsAvailable,
                    //             IsDeleted=newStockItem.IsDeleted,
                    //             MaxCount=0,
                    //             MinCount=0,
                    //             Price=newStockItem.Price,
                    //             PropItems=new(),
                    //             SoldCount=0,
                    //             StockId= 
                    //         };
                    //     }
                    //     else
                    //     {

                    //         item.Price = newStockItem.Price;
                    //         item.Discount = newStockItem.Discount;
                    //         item.Count = newStockItem.Count;
                    //     }
                    // }
                }
            }
            await collection.ReplaceOneAsync(filter, product);
            //log information
            _logger.LogInformation($"product with Id : {product.ProductId}  edited in readDb");


        }

        private async Task<List<PropertyItemReadDb>> GetPropertiesByPropertyType(PropertyCategory item, List<long> props)
        {
            return item.PropertyType switch
            {
                PropertyType.Color => await _colors.Where(w => props.Contains(w.Id)).Select(s => new PropertyItemReadDb
                {
                    Id = s.Id,
                    Name = s.Name,
                    Value = s.Code,
                }).ToListAsync(),
                PropertyType.Size => await _sizes.Where(w => props.Contains(w.Id)).Select(s => new PropertyItemReadDb
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync(),
                PropertyType.Guarantee => await _guarantees.Where(w => props.Contains(w.Id)).Select(s => new PropertyItemReadDb
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync(),
                _ => new(),
            };
        }
    }
}
