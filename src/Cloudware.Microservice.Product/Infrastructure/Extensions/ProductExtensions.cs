using Cloudware.Microservice.Product.Application.Command;
using Cloudware.Microservice.Product.Application.Query;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using GreenPipes.Internals.Extensions;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Cloudware.Microservice.Product.Application.Command.UpdateSellersCollectionCommandConsumer;
namespace Cloudware.Microservice.Product.Infrastructure.Extensions
{
    public static class ProductExtensions
    {
        public static IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection> GetSortFilter(SortBy sortBy, IMongoCollection<ProductItemReadDbCollection> collection)
        {
            IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection> sort = null;
            switch (sortBy)
            {
                case SortBy.Latest:
                    sort = collection.Find(_ => true).SortByDescending(s => s.CreatedDate);
                    //  productItems = productItems.OrderByDescending(o => o.CreatedDate);
                    break;
                case SortBy.SpecialOffer:
                    sort = collection.Find(f => f.IsAvailable == true).SortByDescending(s => s.LastUpdatedDate);
                    break;
                case SortBy.MostViewed:
                    sort = collection.Find(_ => true).SortByDescending(s => s.ViewCount);
                    //productItems = productItems.OrderByDescending(o => o.ViewCount);
                    break;
                case SortBy.Newest:
                    // productItems = productItems.OrderByDescending(o => o.LastUpdatedDate);
                    sort = collection.Find(_ => true).SortByDescending(s => s.LastUpdatedDate);
                    break;
                case SortBy.Hottest:
                    // productItems = productItems.OrderByDescending(o => o.TotalSoldCount);
                    sort = collection.Find(_ => true).SortByDescending(s => s.TotalSoldCount);
                    break;
                case SortBy.MostExpensive:
                    sort = collection.Find(_ => true).SortByDescending(s => s.MinPrice);
                    // productItems = productItems.OrderBy(o => o.StocksItems.Select(si=>si.Price));
                    break;
                case SortBy.LeastExpensive:
                    sort = collection.Find(_ => true).SortBy(s => s.MinPrice);
                    //  productItems = productItems.OrderByDescending(o => o.StocksItems.Select(si => si.Price));
                    // productItems = productItems.Select(s=>s.StocksItems.);
                    break;
                default:
                    break;
            }
            return sort;
        }
        public static List<ProductCategoryReadDb> GetCategoriesChildren(List<ProductCategory> listItem, long parentId)
        {
            return listItem
                    .Where(c => c.ParentId == parentId)
                    .Where(w => !w.IsDeleted)
                    .Select(c => new ProductCategoryReadDb
                    {
                        CategoryId = c.Id,
                        Guid = c.Guid.ToString(),
                        Name = c.Name,
                        ParentId = c.ParentId,
                        Description = c.Description,
                        Icon = c.Icon,
                        Children = GetCategoriesChildren(listItem, c.Id)
                    })
                    .ToList();
        }
        public static List<CustomCategoryCollectionItem> GetCustomCategoriesChildren(List<ProductCategory> listItem, long parentId)
        {
            return listItem
                    .Where(c => c.ParentId == parentId)
                    .Where(w => !w.IsDeleted)
                    .Select(c => new CustomCategoryCollectionItem
                    {
                        Data = new CustomCategoryData
                        {
                            CategoryId = c.Id,
                            Guid = c.Guid.ToString(),
                            ParentId = c.ParentId,
                            Description = c.Description,
                            Icon = c.Icon,
                        },
                        Name = c.Name,
                        Children = GetCustomCategoriesChildren(listItem, c.Id)
                    })
                    .ToList();
        }
        public static List<long> GetCategoriesChildrenIdsRead(List<ProductCategory> listItem, string tenantId, long parentId)
        {
            var res = listItem.Where(w => w.ParentId == parentId && !w.IsDeleted).Select(s => s.Id).ToList();
            foreach (var item in listItem.Where(w => w.ParentId == parentId))
            {
                res.AddRange(GetCategoriesChildrenIdsRead(listItem, tenantId, item.Id));
            }
            return res;

        }


        public static List<long> GetCategoriesChildrenIdsRead(ProductCategoryReadDb listItem, string tenantId, long parentId)
        {
            var res = listItem.Children.Where(w => w.ParentId == parentId).Select(s => s.CategoryId).ToList();
            foreach (var item in listItem.Children.Where(w => w.ParentId == parentId))
            {
                res.AddRange(GetCategoriesChildrenIdsRead(listItem, tenantId, item.CategoryId));
            }
            return res;
        }
        public static ICollection<ProductCategory> GetCategoriesChildrenForSeed(ICollection<ProductCategory> listItem, long parentId, string tenantId)
        {
            return listItem
                    .Where(c => c.ParentId == parentId)
                    .Select(c => new ProductCategory
                    {
                        TenantId = tenantId,
                        Guid = Guid.NewGuid(),
                        CreatedDate = DateTimeOffset.UtcNow,
                        LastUpdatedDate = DateTimeOffset.UtcNow,
                        IsDeleted = false,
                        // Id = c.Id,
                        Name = c.Name,
                        //ParentId = c.ParentId,
                        Children = GetCategoriesChildrenForSeed(listItem, c.Id, tenantId)
                    })
                    .ToList();
        }
        public static List<SellersCollection> MapSellerDtoToSellerCollection(List<SellersDto> sellersDto)
        {
            var sellers = sellersDto.Select(s => new SellersCollection { SellerId = s.UserId, Name = s.UserName, Description = s.UserDescription, ImageUrl = s.UserImageUrl }).ToList();
            return sellers;
        }
        public static List<ProductCategoryNormalizedCollection> GetParents(List<ProductCategoryNormalizedCollection> list, List<ProductCategoryNormalizedCollection> res, long catId)
        {
            var category = list.FirstOrDefault(w => w.CategoryId == catId);
            if (category?.ParentId is null || category is null)
            {
                //res.Add(category);
                return res;
            }
            res.Add(list.FirstOrDefault(w => w.CategoryId == category.ParentId));
            return GetParents(list, res, category.ParentId.Value);
        }
        public static string GetPropertyName(PropertyType propertyType)
        {
            var res = "";
            return propertyType switch
            {
                PropertyType.Color => "رنگ",
                PropertyType.Size => "اندازه",
                PropertyType.Guarantee => "گارانتی",
                _ => "رنگ",
            };
        }
        public static PropertyViewType GetPropertyViewType(PropertyType propertyType)
        {
            var res = PropertyViewType.Color;
            return propertyType switch
            {
                PropertyType.Color => PropertyViewType.Color,
                PropertyType.Size => PropertyViewType.Button,
                PropertyType.Guarantee => PropertyViewType.RadioButton,
                _ => PropertyViewType.Color,
            };
        }
        //  public  static IQueryable GetPropertyQuery(PropertyType propertyType)
        // {
        //     var res =DbSet<Colors;
        //     res = propertyType switch
        //     {
        //         PropertyType.Color => PropertyViewType.Color,
        //         PropertyType.Size => PropertyViewType.RadioButton,
        //         PropertyType.Guarantee => PropertyViewType.RadioButton,
        //         _ => PropertyViewType.Color,
        //     };
        //     return res;
        // }

    }
}
#region comment
//public static string ToKebabCase(string value)
//{
//    // Replace all non-alphanumeric characters with a dash
//    value = Regex.Replace(value, @"[^0-9a-zA-Z]", "-");

//    // Replace all subsequent dashes with a single dash
//    value = Regex.Replace(value, @"[-]{2,}", "-");

//    // Remove any trailing dashes
//    value = Regex.Replace(value, @"-+$", string.Empty);

//    // Remove any dashes in position zero
//    if (value.StartsWith("-")) value = value.Substring(1);

//    // Lowercase and return
//    return value.ToLower();
//}
//public static void AddRequestClientsClw<BaseType>(this IServiceCollectionMediatorConfigurator configurator, params Assembly[] assemblies)
//{

//    var assembly = typeof(BaseType).Assembly;
//    var a = AppDomain.CurrentDomain.GetAssemblies();
//   // var a = Assembly.GetExecutingAssembly();
//   //IEnumerable<Type> types = a.GetExportedTypes()
//    IEnumerable<Type> types = a.SelectMany(s => s.GetExportedTypes())
//        .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(BaseType).IsAssignableFrom(c)).ToList();
//    foreach (Type item in types)
//        configurator.AddRequestClient(item);
//}
//public static void AddMediatorConsumersClw<BaseType>(this IServiceCollectionMediatorConfigurator configurator)
//{
//    var assembly = typeof(BaseType).Assembly;
//    IEnumerable<Type> types = assembly.GetExportedTypes();
//    //.Where(w => w.IsClass && !w.IsAbstract && w.IsPublic && typeof(BaseType).IsAssignableFrom(w));
//    types = types.Where(w => w.IsClass && !w.IsAbstract && w.IsPublic && typeof(BaseType).IsAssignableFrom(w)).ToList();

//    foreach (var item in types)
//        configurator.AddConsumer(item);


//}
//public static void AddBusConsumersClw<BaseType>(this IServiceCollectionBusConfigurator configurator)
//{
//    var assembly = typeof(BaseType).Assembly;
//    IEnumerable<Type> types = assembly.GetExportedTypes()
//        .Where(w => w.IsClass && !w.IsAbstract && w.IsPublic && typeof(BaseType).IsAssignableFrom(w));
//    foreach (var item in types)
//        configurator.AddConsumer(item);


//}
//public static void AddRabbitMqClw(this IServiceCollectionBusConfigurator configurator, IConfiguration Configuration)
//{
//    configurator.AddBusConsumersClw<IBusConsumerType>();
//    configurator.UsingRabbitMq((context, cfg) =>
//     {
//         cfg.Host(new Uri(Configuration.GetValue<string>("MassTransit:Host")), d =>
//         {
//             d.Username(Configuration.GetValue<string>("MassTransit:Username"));
//             d.Password(Configuration.GetValue<string>("MassTransit:Password"));
//         });

//         //  cfg.AddIntegrationConsumersClw<Iinte>(context);
//         cfg.ConfigureEndpoints(context);
//     });
//}
//public static void AddIntegrationConsumersClw<BaseType>(this IRabbitMqBusFactoryConfigurator configurator, IBusRegistrationContext context, params Assembly[] assemblies)
//{
//    var assembly = typeof(BaseType).Assembly;
//    IEnumerable<Type> types = assembly.GetExportedTypes()
//        .Where(w => w.IsClass && !w.IsAbstract && w.IsPublic && typeof(BaseType).IsAssignableFrom(w));
//    var data = ToKebabCase(Assembly.GetExecutingAssembly().GetName().Name);
//    configurator.ReceiveEndpoint(data, e =>
//     {
//         foreach (var item in types)
//             e.ConfigureConsumer(context, item.GetType());
//     });
//}
//public static void AddMassTransitClw(this IServiceCollection services, IConfiguration Configuration)
//{
//    services.AddMassTransit(x => x.AddRabbitMqClw(Configuration));
//}
//public static void AddMassTransitMediatorClw<MediatorConsumerType, RequestType>(this IServiceCollection services)
//{
//    services.AddMediator(x =>
//    {
//        //consumers
//        // x.AddMediatorConsumersClw<MediatorConsumerType>();
//        // requestClients
//        var ab = typeof(IRequestType).Assembly;
//        x.AddRequestClientsClw<IRequestType>(ab);
//    });
//}
#endregion

// var data = listItem.Children.Where(a => a.ParentId == parentId).Select(w => w.CategoryId).ToList();
// foreach (var item in listItem.Children.Where(a => a.ParentId == parentId))
// {
//     data.AddRange(GetCategoriesChildrenIdsRead(item, tenantId, item.CategoryId));
// }
// return data;
//namespace Cloudware.Utilities.BusTools
//{
//    public static class BusExtensions
//    {
//        public static void AddRequestClientsClw<RequestType>(this IServiceCollectionMediatorConfigurator configurator, params Assembly[] assemblies)
//        {
//            var assembly = typeof(RequestType).Assembly;

//            IEnumerable<Type> types = assemblies.SelectMany(s => s.GetExportedTypes())
//            //IEnumerable<Type> types = assembly.GetExportedTypes()
//                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(RequestType).IsAssignableFrom(c)).ToList();
//            foreach (Type item in types)
//                configurator.AddRequestClient(item);
//        }
//        public static void AddMediatorConsumersClw<MediatorConsumerType>(this IServiceCollectionMediatorConfigurator configurator, params Assembly[] assemblies)
//        {
//            var assembly = typeof(MediatorConsumerType).Assembly;

//            IEnumerable<Type> types = assemblies.SelectMany(s => s.GetExportedTypes())
//                //  IEnumerable<Type> types = assemblies.GetExportedTypes()
//                .Where(w => w.IsClass && !w.IsAbstract && w.IsPublic && typeof(MediatorConsumerType).IsAssignableFrom(w)).ToList();
//            foreach (var item in types)
//                configurator.AddConsumer(item);


//        }
//        public static void AddBusConsumersClw<BusConsumerType>(this IServiceCollectionBusConfigurator configurator, params Assembly[] assemblies)
//        {
//            var assembly = typeof(BusConsumerType).Assembly;

//            IEnumerable<Type> types = assemblies.SelectMany(s => s.GetExportedTypes())
//                //  IEnumerable<Type> types = assembly.GetExportedTypes()
//                .Where(w => w.IsClass && !w.IsAbstract && w.IsPublic && typeof(BusConsumerType).IsAssignableFrom(w)).ToList();
//            foreach (var item in types)
//                configurator.AddConsumer(item);


//        }
//        public static void AddRabbitMqClw<BusMultiConsumerType>(this IServiceCollectionBusConfigurator configurator, string hostUrl, string hostUserName, string hostPassword, params Assembly[] assemblies)
//        {
//            configurator.UsingRabbitMq((context, cfg) =>
//            {
//                cfg.Host(new Uri(hostUrl), d =>
//                {
//                    d.Username(hostUserName);
//                    d.Password(hostPassword);
//                });
//                cfg.AddBusMultiConsumersClw<BusMultiConsumerType>(context, assemblies);

//                cfg.ConfigureEndpoints(context);
//            });
//        }
//        public static void AddBusRequestClientsClw<BusRequestClientType>(this IServiceCollectionBusConfigurator configurator, params Assembly[] assemblies)
//        {
//            IEnumerable<Type> types = assemblies.SelectMany(s => s.GetExportedTypes())
//                .Where(w => w.IsClass && !w.IsAbstract && w.IsPublic && typeof(BusRequestClientType).IsAssignableFrom(w)).ToList();
//            foreach (var item in types)
//                configurator.AddRequestClient(item);
//        }

//        public static void AddBusMultiConsumersClw<BusMultiConsumerType>(this IRabbitMqBusFactoryConfigurator configurator, IBusRegistrationContext context, params Assembly[] assemblies)
//        {
//            var assembly = typeof(BusMultiConsumerType).Assembly;

//            IEnumerable<Type> types = assemblies.SelectMany(s => s.GetExportedTypes())
//                          //  IEnumerable<Type> types = a.GetExportedTypes()
//                          .Where(w => w.IsClass && !w.IsAbstract && w.IsPublic && typeof(BusMultiConsumerType).IsAssignableFrom(w)).ToList();
//            var data = ToKebabCase(Assembly.GetExecutingAssembly().GetName().Name);
//            configurator.ReceiveEndpoint(data, e =>
//            {
//                foreach (var item in types)
//                    e.ConfigureConsumer(context, item);

//                //e.confi
//            });
//        }
//        public static void AddMassTransitClw<BusConsumerType, BusMultiConsumerType, BusRequestClientType>(this IServiceCollection services, string hostUrl, string hostUserName, string hostPassword, params Assembly[] assemblies)
//        {
//            services.AddMassTransit(x =>
//            {
//                x.AddBusConsumersClw<BusConsumerType>(assemblies);
//                x.AddBusRequestClientsClw<BusRequestClientType>(assemblies);
//                x.SetKebabCaseEndpointNameFormatter();
//                x.AddRabbitMqClw<BusMultiConsumerType>(hostUrl, hostUserName, hostPassword, assemblies);
//            }

//            );
//        }
//        public static void AddMassTransitMediatorClw<MediatorConsumerType, RequestType>(this IServiceCollection services, params Assembly[] assemblies)
//        {
//            services.AddMediator(x =>
//            {
//                //consumers
//                x.AddMediatorConsumersClw<MediatorConsumerType>(assemblies);
//                // requestClients
//                x.AddRequestClientsClw<RequestType>(assemblies);
//            });
//        }
//        public static string ToKebabCase(string value)
//        {
//            // Replace all non-alphanumeric characters with a dash
//            value = Regex.Replace(value, @"[^0-9a-zA-Z]", "-");

//            // Replace all subsequent dashes with a single dash
//            value = Regex.Replace(value, @"[-]{2,}", "-");

//            // Remove any trailing dashes
//            value = Regex.Replace(value, @"-+$", string.Empty);

//            // Remove any dashes in position zero
//            if (value.StartsWith("-")) value = value.Substring(1);

//            // Lowercase and return
//            return value.ToLower();
//        }
//    }

//}
