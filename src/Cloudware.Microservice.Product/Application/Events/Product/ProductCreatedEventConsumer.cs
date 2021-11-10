using Cloudware.Microservice.Product.Application.Events.Test;
using Cloudware.Microservice.Product.Application.Query;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Infrastructure.Repositories;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.People;
using Cloudware.Utilities.Formbuilder.Entities;
using Cloudware.Utilities.Formbuilder.Services;
using DnsClient.Internal;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events
{
    public class ProductCreatedEventConsumer : IConsumer<ProductCreatedEvent>, IMediatorConsumerType
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductItem> _productItems;
        private readonly DbSet<Color> _colors;
        private readonly DbSet<Guarantee> _guarantees;
        private readonly DbSet<Size> _sizes;
        private readonly DbSet<PropertyCategory> _propertyCategories;
        private readonly IProductReadDbContext _context;
        private readonly IRecordsService _recordsService;
        private readonly IPropertyService _propertyService;
        private readonly IFormManagementService _formManagementService;
        private readonly ILogger<ProductCreatedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        public ProductCreatedEventConsumer(IUnitOfWork uow, ILogger<ProductCreatedEventConsumer> logger, IProductReadDbContext context, IRecordsService recordsService, IFormManagementService formManagementService, IPropertyService propertyService, IPublishEndpoint publishEndpoint)
        {
            _uow = uow;
            _productItems = _uow.Set<ProductItem>();
            _colors = _uow.Set<Color>();
            _guarantees = _uow.Set<Guarantee>();
            _sizes = _uow.Set<Size>();
            _propertyCategories = _uow.Set<PropertyCategory>();
            _logger = logger;
            _context = context;
            _recordsService = recordsService;
            _formManagementService = formManagementService;
            _propertyService = propertyService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {

            List<TechnicalPropertyReadDb> technicalProperties = new();
            if (!string.IsNullOrEmpty(context.Message.ProductItem.TechnicalPropertyFormId))
            {
                var records = await _recordsService.GetRecordById(context.Message.ProductItem.TechnicalPropertyRecordId, context.Message.ProductItem.TenantId);
                var form = await _formManagementService.GetForm(context.Message.ProductItem.TechnicalPropertyFormId);
                foreach (var item in records.RecordsData)
                {
                    switch (item.FieldType)
                    {
                        case FieldType.BaseText:
                            {
                                technicalProperties.Add(new TechnicalPropertyReadDb
                                {
                                    Name = form.Groups.SelectMany(s => s.FormFields).Where(w => w.Id == item.FieldId).Select(s => s.DisplayName).FirstOrDefault(),
                                    Value = item.Value,
                                });
                            }
                            break;
                        case FieldType.CheckBox:
                            {
                                technicalProperties.Add(new TechnicalPropertyReadDb
                                {
                                    Name = form.Groups.SelectMany(s => s.FormFields).Where(w => w.Id == item.FieldId).Select(s => s.DisplayName).FirstOrDefault(),
                                    Value = item.Value == "true" ? "دارد" : "ندارد",
                                });
                            }
                            break;
                        case FieldType.Radio:
                            {

                                technicalProperties.Add(new TechnicalPropertyReadDb
                                {
                                    Name = form.Groups.SelectMany(s => s.FormFields).Where(w => w.Id == item.FieldId).Select(s => s.DisplayName).FirstOrDefault(),
                                    Value = form.Groups.SelectMany(s => s.FormFields).Where(w => w.Id == item.FieldId).SelectMany(s => s.SelectionRow).Where(w => w.Id == item.SelectionIds.FirstOrDefault()).Select(s => s.Value).FirstOrDefault(),
                                });
                            }
                            break;
                            // default: break;
                    }
                }

            }
            var categoryProperty = await _propertyCategories.Where(w => w.CategoryId == context.Message.CreateProductCommand.CategoryId).ToListAsync();
            var properties = new List<(PropertyType, List<PropertyItemReadDb>)>();

            foreach (PropertyCategory item in categoryProperty)
            {
                var props = context.Message.ProductItem.StocksItems.SelectMany(s => s.StockPropertyItems).Where(w => w.PropertyType == item.PropertyType).Select(s => s.PropertyId).ToList();
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
            var productItemDto = await _productItems
                  .Where(prodId => prodId.Id == context.Message.ProductItem.Id)
                  .Select(s => new ProductItemReadDbCollection
                  {
                      ProductId = s.Id,
                      Name = s.Name,
                      Address = context.Message.CreateProductCommand.Address,
                      City = context.Message.CreateProductCommand.City,
                      CityId = context.Message.CreateProductCommand.CityId,
                      Mobile = context.Message.CreateProductCommand.Mobile,
                      Province = context.Message.CreateProductCommand.Province,
                      ProvinceId = context.Message.CreateProductCommand.ProvinceId,
                      Description = s.Description,
                      Story = s.Story,
                      UserId = s.UserId,
                      TenantId = s.TenantId,
                      ImageUrl = s.Image,
                      Code = s.Code,
                      IsDeleted = false,
                      Guid = s.Guid.ToString(),
                      Weight = s.Weight,
                      CreatedDate = s.CreatedDate.DateTime,
                      LastUpdatedDate = s.LastUpdatedDate.DateTime,
                      Dimensions = s.Dimensions,
                      BrandId = s.BrandId,
                      BrandName = s.Brand.Name,
                      TotalCount = s.StocksItems.Select(s => s.Count).Sum(),
                      MaxPrice = s.StocksItems.Where(w => !w.IsDeleted).OrderByDescending(od => od.Price).Select(sp => sp.Price).FirstOrDefault(),
                      MinPrice = s.StocksItems.Where(w => !w.IsDeleted).OrderBy(oa => oa.Price).Select(sp => sp.Price).FirstOrDefault(),
                      CategoryId = s.CategoryId,
                      CategoryName = s.Category.Name,
                      UserDescription = context.Message.CreateProductCommand.UserDescription,
                      UserImageUrl = context.Message.CreateProductCommand.UserImageUrl,
                      UserName = context.Message.CreateProductCommand.UserName,
                      ImageUrls = s.ImageUrls.Select(img => img.Url).ToList(),
                      TechnicalPropertyFormId = s.TechnicalPropertyFormId ?? "",
                      TechnicalPropertyRecordId = s.TechnicalPropertyRecordId ?? "",
                      SupplierId = s.SupplierId,
                      TechnicalPropertiesReadDb = s.TechnicalPropertyFormId == null ? s.TechnicalProperties.Select(tp => new TechnicalPropertyReadDb
                      {
                          Name = tp.Name,
                          Value = tp.Value
                      }).ToList() : technicalProperties,
                      //TechnicalPropertiesReadDb = s.TechnicalProperties.Select(tp => new TechnicalPropertyReadDb { Name = tp.Name, Value = tp.Value }).ToList(),
                      IsAvailable = context.Message.CreateProductCommand.IsActive,
                      // IsAvailable = !s.StocksItems.Any(a => a.Count == 0),
                      StockItemsDto = s.StocksItems.Where(w => !w.IsDeleted).Select(g =>
                       new StockItemReadDb
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
                       }).ToList(),
                      PropertiesDto = propertyReadDb ?? new()
                      //   PropertiesDto = s.StocksItems.SelectMany(s => s.StockPropertyItems).Select(ps =>
                      //       new PropertyReadDb
                      //       {
                      //           PropertyId = ps.Id,
                      //           Name = ProductExtensions.GetPropertyName(ps.PropertyType),
                      //           PropertyType = ps.PropertyType,
                      //           PropertyViewType=
                      //           PropertyItemDtos = ps..Select(spit =>
                      //           new PropertyItemReadDb
                      //           {
                      //               Id = spit.Id,
                      //               Name = spit.Name,
                      //               Value = spit.Value

                      //           }).ToList()
                      //       }).ToList()
                  }).FirstOrDefaultAsync();

            await _context.ProductItemsDataCollection.InsertOneAsync(productItemDto);

            var getPeopleByIdIntegrationEvent = new GetPeopleByIdIntegrationEvent(context.Message.ProductItem.SupplierId);
            await _publishEndpoint.Publish(getPeopleByIdIntegrationEvent, x => x.CorrelationId = context.Message.ProductItem.Guid);

            // _logger.LogDebug($"Product : {productItemDto} added to readDb");
        }


    }
}
