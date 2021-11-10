using AutoMapper;
using Cloudware.Microservice.Product.Application.Events;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.People;
using Cloudware.Utilities.Formbuilder.Services;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Cloudware.Microservice.Product.Application.Command
{
    public class CreateProductCommandConsumer : IConsumer<CreateProductCommand>, IMediatorConsumerType
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _uow;
        private readonly IRecordsService _recordsService;
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly IFormManagementService _formManagementService;
        private readonly DbSet<ProductItem> _productItems;
        private readonly DbSet<TechnicalProperty> _technicalProperties;
        private readonly DbSet<ImageUrlItems> _imageUrlItems;
        private readonly DbSet<StockItem> _stockItems;
        private readonly DbSet<PropertyCategory> _propertyCategories;

        private readonly DbSet<StockProperty> _stockPropertyItems;
        private readonly DbSet<ProductCategory> _productCategories;
        private readonly DbSet<ProductBrand> _productBrands;
        private readonly ILogger<CreateProductCommandConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateProductCommandConsumer(IUnitOfWork unitOfWork,
            IMediator mediator,
            ILogger<CreateProductCommandConsumer> logger,
            IRecordsService recordsService,
            IFormManagementService formManagementService,
            IProductReadDbContext productReadDbContext,
            IPublishEndpoint publishEndpoint)
        {
            _uow = unitOfWork;
            _mediator = mediator;
            _productItems = _uow.Set<ProductItem>();
            _technicalProperties = _uow.Set<TechnicalProperty>();
            _imageUrlItems = _uow.Set<ImageUrlItems>();
            _stockItems = _uow.Set<StockItem>();
            _propertyCategories = _uow.Set<PropertyCategory>();

            _stockPropertyItems = _uow.Set<StockProperty>();
            _productCategories = _uow.Set<ProductCategory>();
            _productBrands = _uow.Set<ProductBrand>();
            _logger = logger;
            _recordsService = recordsService;
            _formManagementService = formManagementService;
            _productReadDbContext = productReadDbContext;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<CreateProductCommand> context)
        {
            try
            {
                if (context.Message.Image is null)
                {
                    throw new AppException(5056, System.Net.HttpStatusCode.BadRequest, "image can not be null!!!");
                }
                var categoryId = context.Message.CategoryId ?? 0;
                var category = await _productCategories.FirstOrDefaultAsync(f => f.Id == categoryId);
                if (category == null)
                {
                    categoryId = await _productCategories.Where(f => f.TenantId == context.Message.TenantId).Select(s => s.Id).FirstOrDefaultAsync();
                    categoryId = categoryId == 0 ? 1 : categoryId;
                }
                var brandId = context.Message.BrandId ?? 0;
                var brand = await _productBrands.FirstOrDefaultAsync(f => f.Id == brandId);
                if (brand == null)
                {
                    brandId = await _productBrands.Where(f => f.TenantId == context.Message.TenantId).Select(s => s.Id).FirstOrDefaultAsync();
                    brandId = brandId == 0 ? 1 : brandId;
                }
                var categoryProperties = await _propertyCategories.Where(w => w.CategoryId == categoryId).Select(s => s.PropertyType).ToListAsync();
                if (categoryProperties.Count > 0)
                {
                    // var props = context.Message.StocksItems..ToList();
                    foreach (var item in context.Message.StocksItems)
                    {
                        if (item.PropItems.Select(s => s.PropertyType).Except(categoryProperties).Any())
                            throw new AppException(5056, System.Net.HttpStatusCode.BadRequest, "category properties does not match entered data!!!");

                    }
                }
                var formId = await _productReadDbContext.ProductCategoryNormalizedCollection.Find(f => f.CategoryId == context.Message.CategoryId && f.TenantId == context.Message.TenantId).Project(p => p.FormId).FirstOrDefaultAsync();

                var productItem = new ProductItem
                {
                    TenantId = context.Message.TenantId ?? "",
                    UserId = context.Message.UserId ?? "",
                    BrandId = brandId,
                    CategoryId = categoryId,
                    Image = context.Message.Image,
                    Description = context.Message.Description ?? "",
                    Code = context.Message.Code ?? "",
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastUpdatedDate = DateTimeOffset.UtcNow,
                    IsActive = context.Message.IsActive,
                    Name = context.Message.Name,
                    Dimensions = context.Message.Dimensions ?? "",
                    Weight = context.Message.Weight ?? 0,
                    Story = context.Message.Story ?? "",
                    Guid = Guid.NewGuid(),
                    TechnicalPropertyFormId = formId,
                    SupplierId = context.Message.SupplierId
                };
                var product = await _productItems.AddAsync(productItem);


                if (context.Message.TechnicalProperties != null)
                {
                    ICollection<TechnicalProperty> technicalProperties = (from item in context.Message.TechnicalProperties
                                                                          select new TechnicalProperty
                                                                          {
                                                                              Name = item.Name,
                                                                              Value = item.Value,
                                                                              ProductItem = productItem
                                                                          }).Where(w => w.Value != null).ToList();

                    await _technicalProperties.AddRangeAsync(technicalProperties);
                }
                var images = context.Message.ImageUrls;
                // images.Add(context.Message.Image);
                ICollection<ImageUrlItems> imageUrlItems = (from item in images
                                                            select new ImageUrlItems
                                                            {
                                                                Url = item,
                                                                ProductItem = productItem
                                                            }).Where(w => w.Url != null).ToList();
                images = images.Distinct().ToList();
                await _imageUrlItems.AddRangeAsync(imageUrlItems);

                // FORM RECORDS
                await _uow.SaveChangesAsync();
                if (formId is not null)
                {
                    var form = await _formManagementService.GetForm(formId, context.Message.TenantId);
                    var record = context.Message.TechnicalProperty.AddRecord;
                    record.FormId = formId;
                    record.TenantId = context.Message.TenantId;
                    record.ServiceId = context.Message.TenantId;
                    record.UserId = context.Message.UserId;
                    record.ObjectId = productItem.Id.ToString();
                    var recordId = await _recordsService.AddRecord(record);

                    productItem.TechnicalPropertyRecordId = recordId;
                    _productItems.Update(productItem);
                    await _uow.SaveChangesAsync();
                }

                //stock
                ICollection<StockItem> stockItems = new List<StockItem>();
                List<StockProperty> stockPropertyItems = new();

                foreach (var item in context.Message.StocksItems)
                {
                    stockPropertyItems = new();
                    if (item.PropItems != null && item.PropItems.Count != 0)
                    {
                        foreach (var stockPropertyId in item.PropItems)
                        {
                            if (stockPropertyId.PropertyId is not null)
                                stockPropertyItems.Add(new StockProperty { PropertyId = Int64.Parse(stockPropertyId.PropertyId), PropertyType = stockPropertyId.PropertyType });
                        }
                    }

                    var stockItem = new StockItem
                    {
                        Count = item.Count,
                        MaxCount = item.MaxCount,
                        MinCount = item.MinCount,
                        CreatedDate = DateTimeOffset.UtcNow,
                        LastUpdatedDate = DateTimeOffset.UtcNow,
                        Price = item.Price,
                        Discount = item.Discount,
                        //SoldCount = 0,
                        ProductItem = productItem,
                        IsAvailable = item.IsAvailable ?? true
                        ,
                        StockPropertyItems = stockPropertyItems ?? new()
                    };

                    stockItems.Add(stockItem);
                }

                await _stockItems.AddRangeAsync(stockItems);

                await _uow.SaveChangesAsync();
                _logger.LogDebug($"Product : {productItem} created!");
                //  await _mediator.Publish(new ProductCreatedEvent(productItem.Id));
                await _mediator.Publish(new ProductCreatedEvent(productItem, context.Message));

                await context.RespondAsync(new CreateProductCommandResponse(productItem.Id, productItem.Guid.ToString()));
            }
            catch (Exception e)
            {
                _logger.LogError("Product Creation Failed With Error : " + e.Message);
                await context.RespondAsync(new CreateProductCommandResponse(0, ""));
                throw new AppException(500, e.Message, e);
            }
        }

    }



    public class CreateProductCommandResponse
    {
        public long ProductId { get; set; }
        public string Guid { get; set; }

        public CreateProductCommandResponse(long productId, string guid)
        {
            ProductId = productId;
            Guid = guid;
        }
    }
}
// TotalSoldCount = 0,
// ViewCount = 0,
// TechnicalPropertyRecordId = recordId,
//technical
// List<TechnicalProperty> technicalProperties1 = new();
// var tech = await _recordsService.GetRecordById(recordId, context.Message.TechnicalProperty.AddRecord.ServiceId);
// var form = await _formManagementService.GetForm(tech.FormId);

// foreach (var item in tech.RecordsData)
// {

//     technicalProperties1.Add(new TechnicalProperty
//     {
//         Name = form.Groups.SelectMany(s => s.FormFields).Where(w => w.Id == item.FieldId).Select(s => s.DisplayName).FirstOrDefault(),
//         Value = item.Value,
//         ProductItem = productItem
//     });
// }
// public class PropertyItemIdSet
// {
//     public PropertyItem propertyItem { get; set; }
//     public long Id { get; set; }
// }
//stockPropertyItems= propertyItemIdSet.Where(f => item.PropItems.Contains(f.Id)).Select(s => new StockPropertyItem { PropItemId = s.propertyItem.Id }).ToList();
// stockPropertyItems.Add(new StockPropertyItem { PropItemId = propertyItemIdSet.FirstOrDefault(f =>item.PropItems.Contains(f.Id)).propertyItem.Id });
//property items
// List<PropertyItemCommandDto> propertyItems = new();
// foreach (var item in context.Message.Properties)
// {
//     propertyItems.AddRange(item.PropertyItems.Select(s => new PropertyItemCommandDto { Id = s.Id, Name = s.Name, Value = s.Value }).ToList());
// }
// }
// else
// {
// var stockItem = new StockItem
// {
//     Count = context.Message.StocksItems.Select(s => s.Count).FirstOrDefault(),
//     CreatedDate = DateTimeOffset.UtcNow,
//     LastUpdatedDate = DateTimeOffset.UtcNow,
//     Price = context.Message.StocksItems.Select(s => s.Price).FirstOrDefault(),
//     ProductItem = productItem
// };
// await _stockItems.AddAsync(stockItem);
// }


//property
// if (context.Message?.Properties.Count != 0)
// {

// ICollection<Property> properties = new List<Property>();


// var propertyItemIdSet = new List<PropertyItemIdSet>();
// foreach (var property in context.Message.Properties)
// {
//     List<PropertyItem> propertyItemList = new();
//     foreach (var propertyitems in property.PropertyItems)
//     {
//         var propertyItem = new PropertyItem
//         {
//             Name = propertyitems.Name,
//             Value = propertyitems.Value,
//         };
//         propertyItemList.Add(propertyItem);
//         propertyItemIdSet.Add(new PropertyItemIdSet { Id = propertyitems.Id, propertyItem = propertyItem });
//     }
//     var propertyData = new Property { Name = property.Name, PropItems = propertyItemList, ProductItem = productItem, PropertyType = property.PropertyType };
//     properties.Add(propertyData);
// }
// await _properties.AddRangeAsync(properties);
// await _uow.SaveChangesAsync();