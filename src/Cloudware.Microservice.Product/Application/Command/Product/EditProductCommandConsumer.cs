using Cloudware.Microservice.Product.Application.Events;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;
using Cloudware.Utilities.Contract.Abstractions;
using System.Collections.Concurrent;
using Cloudware.Utilities.Common.Exceptions;
using System.Net;

namespace Cloudware.Microservice.Product.Application.Command
{
    public class EditProductCommandConsumer : IConsumer<EditProductCommand>, IMediatorConsumerType
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductItem> _productItems;
        private readonly DbSet<ImageUrlItems> _imageUrlItems;
        private readonly DbSet<ProductCategory> _productCategory;
        private readonly DbSet<TechnicalProperty> _technicalProperties;
        private readonly DbSet<ProductBrand> _productBrand;
        private readonly DbSet<StockItem> _stockItem;
        private readonly ILogger<EditProductCommandConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public EditProductCommandConsumer(IUnitOfWork uow, ILogger<EditProductCommandConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _uow = uow;
            _productItems = _uow.Set<ProductItem>();
            _imageUrlItems = _uow.Set<ImageUrlItems>();
            _productCategory = _uow.Set<ProductCategory>();
            _productBrand = _uow.Set<ProductBrand>();
            _stockItem = _uow.Set<StockItem>();
            _technicalProperties = _uow.Set<TechnicalProperty>();
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<EditProductCommand> context)
        {
            var productItem = await _productItems.FirstOrDefaultAsync(f => f.Guid.ToString().ToLower() == context.Message.Id.ToString().ToLower());
            if (productItem.TenantId != context.Message.TenantId)
            {
                throw new AppException(5056, HttpStatusCode.BadRequest, "this product does not belong to your tenant");
            }
            context.Message.StocksItems ??= new List<StockItemEditCommandDto>();
            if (productItem.TechnicalPropertyRecordId is not null && context.Message?.OldRecordId != productItem.TechnicalPropertyRecordId)
            {
                throw new AppException(5056, HttpStatusCode.BadRequest, "OldRecordId does not match !!!");
            }
            var stockItems = await _stockItem.Where(w => w.ProductId == productItem.Id).ToListAsync();
            stockItems = stockItems.Select(s => { s.IsDeleted = true; return s; }).ToList();

            // var props = await _properties.Where(w => w.ProductItemId == context.Message.Id).ToListAsync();
            // props = props.Select(s =>
            // {
            //     s.IsDeleted = true;
            //     s.PropItems = s.PropItems.Select(ss => { ss.IsDeleted = true; return ss; }).ToList();
            //     return s;
            // }).ToList();
            _imageUrlItems.RemoveRange(_imageUrlItems.Where(x => x.ProductItemId == productItem.Id).AsEnumerable());
            await _imageUrlItems.AddRangeAsync((from item in context.Message.ImageUrls
                                                select new ImageUrlItems(item, productItem.Id)).ToList());

            productItem.Image = context.Message.Image;
            //productItem.ImageUrls = (from item in context.Message.ImageUrls
            //  select new ImageUrlItems(item, context.Message.Id)).ToList();
            productItem.IsActive = context.Message.IsAvailable;
            productItem.Name = context.Message.Name;
            productItem.Story = context.Message.Story;
            productItem.Description = context.Message.Description;

            productItem.LastUpdatedDate = DateTimeOffset.UtcNow;
            //TODO: ///////////////////////////
            productItem.BrandId = context.Message.BrandId ?? 1;
            productItem.CategoryId = context.Message.CategoryId ?? 1;
            //! ////////////////////////////////
            productItem.Code = context.Message.Code;
            productItem.SupplierId = context.Message.SupplierId;

            //basic shop
            if (context.Message.TechnicalPropertiesDto.Count != 0 && context.Message.TechnicalPropertiesDto is not null)
            {
                _technicalProperties.RemoveRange(_technicalProperties.Where(x => x.ProductItemId == productItem.Id).AsEnumerable());
                // var technicalProps = await _technicalProperties.Where(w => w.ProductItemId == productItem.Id).ToListAsync();
                var upTech = new List<TechnicalProperty>();
                foreach (var item in context.Message.TechnicalPropertiesDto)
                {
                    if (item is not null)
                    {
                        var tech = new TechnicalProperty
                        {
                            Name = item.Name,
                            Value = item.Value,
                            ProductItem = productItem,
                            ProductItemId = productItem.Id
                        };
                        upTech.Add(tech);
                    }
                }
                await _technicalProperties.AddRangeAsync(upTech);
            }


            var newStocks = new List<StockItem>();
            foreach (var (item, newStock) in from newStock in context.Message.StocksItems
                                             let item = newStock.Id == null ? stockItems.FirstOrDefault() : stockItems.FirstOrDefault(w => w.Id ==Int64.Parse(newStock.Id) )
                                             select (item, newStock))
            {
                if (item == null)
                {
                    //List<StockProperty> stockPropertyItems = new();

                    //foreach (var stockPropertyId in newStock.PropItems)
                    //{
                    //    stockPropertyItems.Add(new StockProperty { PropertyId = stockPropertyId.PropertyId,PropertyType= });
                    //}
                    newStock.PropItems = newStock.PropItems.Where(w =>!string.IsNullOrEmpty (w.PropertyId)).ToList();
                    var stockItem = new StockItem
                    {
                        Count = newStock.Count,
                        MaxCount = 0,
                        MinCount = 0,
                        CreatedDate = DateTimeOffset.UtcNow,
                        LastUpdatedDate = DateTimeOffset.UtcNow,
                        Price = newStock.Price,
                        Discount = newStock.Discount,
                        IsAvailable = newStock.IsAvailable,
                        ProductItem = productItem,
                        StockPropertyItems = newStock.PropItems.Count!=0? newStock.PropItems.Select(s => new StockProperty { PropertyId = Int64.Parse(s.PropertyId), PropertyType = s.PropertyType }).ToList():null
                    };

                    newStocks.Add(stockItem);
                    continue;
                    //add new 
                }
                //update
                item.Discount = newStock.Discount;
                item.Count = newStock.Count;
                item.Price = newStock.Price;
                item.LastUpdatedDate = DateTimeOffset.UtcNow;
                item.IsDeleted = newStock.IsDeleted;
                item.IsAvailable = newStock.IsAvailable;
                
                stockItems.Add(item);
            }
            stockItems.AddRange(newStocks);
            _stockItem.UpdateRange(stockItems);


            _productItems.Update(productItem);
            await _uow.SaveChangesAsync();
            // await _mediator.Publish(new ProductEditedEvent(productItem));
            // log information
            await _publishEndpoint.Publish(new ProductEditedEvent(context.Message,productItem,productItem.Id));
            await context.RespondAsync(new EditProductCommandResponse(productItem.Id));
            _logger.LogInformation($"Product with Id: { context.Message.Id} edited in writeDb");
        }

    }
    public class EditProductCommandResponse
    {
        public long ProductId { get; set; }

        public EditProductCommandResponse(long productId)
        {
            ProductId = productId;
        }
    }
}
#region comment
#region comment
//property 
//foreach (var item in context.Message.Properties)
//{
//    if (item.Id==0)
//    {
//        //create new property
//    }
//    else
//    {
//        var property = await _property.FirstOrDefaultAsync(f => f.Id == item.Id);
//        if (property==null)
//        {
//            //not found
//        }
//        else {
//        property.Name = item.Name;
//        foreach (var propItem in item.PropertyItems)
//        {
//            var propertyItemData = await _propertyItem.FirstOrDefaultAsync(f => f.Id == propItem.Id);
//            if (propertyItemData==null)
//            {
//                //create
//                await _propertyItem.AddAsync(new PropertyItem {Name=propItem.Name,Value=propItem.Value,Prop=property});
//            }
//            else
//            {
//                //update

//                propertyItemData.Name = propItem.Name;
//                propertyItemData.Value = propItem.Value;

//            }
//        } 
//        }
//    }
//}
// productItem.StocksItems
//if (productItem.CategoryId!=context.Message.CategoryId)
//{
//    var category = await _productCategory.FirstOrDefaultAsync(f => f.Id == context.Message.CategoryId);
//    productItem.Category = category;
//}
//if (productItem.BrandId   != context.Message.BrandId)
//{
//    var brand = await _productBrand.FirstOrDefaultAsync(f => f.Id == context.Message.BrandId);
//    productItem.Brand = brand;
//}

//productItem.PropertyItems = context.Message.PropertyItems;
//productItem.StocksItems = context.Message.StocksItems; 
#endregion



// var deleteProps = context.Message.Properties.Select(s => s.Id.Value).Except(props.Select(s => s.Id).ToList());
// var propertyItemIdSets = new List<PropertyItemIdSet>();
// ICollection<Property> properties = new List<Property>();
// var newProps = false;

// foreach (var (item, newProp) in from newProp in context.Message.Properties
//                                 let item = props.FirstOrDefault(w => w?.Id == newProp.Id)
//                                 select (item, newProp))
// {
//     if (item == null)
//     {
//         newProps = true;
//         //add new 
//         List<PropertyItem> propertyItemList = new();
//         foreach (var propertyitems in newProp.PropertyItems)
//         {
//             var propertyItem = new PropertyItem
//             {
//                 Name = propertyitems.Name,
//                 Value = propertyitems.Value,
//             };
//             propertyItemList.Add(propertyItem);
//             propertyItemIdSets.Add(new PropertyItemIdSet
//             {
//                 Id = propertyitems.Id,
//                 propertyItem = propertyItem
//             });
//         }
//         var propertyData = new Property
//         {
//             Name = newProp.Name,
//             PropItems = propertyItemList,
//             ProductItem = productItem,
//             PropertyType = newProp.PropertyType
//         };
//         properties.Add(propertyData);
//         continue;
//     }
//     //update
//     List<PropertyItem> propertyUpdateItemList = new();
//     foreach (var propertyitems in newProp.PropertyItems)
//     {
//         var updatePropItem = item.PropItems.FirstOrDefault(f => f.Id == propertyitems.Id);


//         updatePropItem.Name = propertyitems.Name;
//         updatePropItem.Value = propertyitems.Value;
//         updatePropItem.IsDeleted = false;

//         propertyUpdateItemList.Add(updatePropItem);

//         propertyItemIdSets.Add(new PropertyItemIdSet
//         {
//             Id = propertyitems.Id,
//             propertyItem = updatePropItem
//         });
//     }
//     item.Name = newProp.Name;
//     item.PropertyType = newProp.PropertyType;
//     item.PropItems = propertyUpdateItemList;
//     item.IsDeleted = false;
//     properties.Add(item);

// }
// if (newProps)
// {
//     await _properties.AddRangeAsync(properties);
// }
// _properties.UpdateRange(properties);
// await _uow.SaveChangesAsync();
#endregion