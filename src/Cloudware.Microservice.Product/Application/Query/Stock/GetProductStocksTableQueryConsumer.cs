using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Query.Category;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Table;
using MassTransit;
using MassTransit.Mediator;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Cloudware.Microservice.Product.Application.Query.Stock
{
    public class GetProductStocksTableQueryConsumer : IConsumer<GetProductStocksTableQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly ITableDataManager _tableDataManager;
        private readonly IMediator _mediator;
        private readonly IPaginationService _paginationService;

        public GetProductStocksTableQueryConsumer(IProductReadDbContext productReadDbContext, ITableDataManager tableDataManager, IMediator mediator, IPaginationService paginationService)
        {
            _productReadDbContext = productReadDbContext;
            _tableDataManager = tableDataManager;
            _mediator = mediator;
            _paginationService = paginationService;
        }

        public async Task Consume(ConsumeContext<GetProductStocksTableQuery> context)
        {
            var collection = _productReadDbContext.ProductItemsDataCollection.AsQueryable();
            var product = await collection.FirstOrDefaultAsync(f => f.ProductId == context.Message.ProductId && f.TenantId == context.Message.TenantId);
            var catPropsReq = _mediator.CreateRequestClient<GetCategoryPropertiesQuery>();
            var catProps = await catPropsReq.GetResponse<GetCategoryPropertiesQueryResponse>(new GetCategoryPropertiesQuery(product.CategoryId, product.TenantId));

            var table = new TableData();

            var fields = GetFields(catProps);
            var props = typeof(StockItemReadDb).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            var rows = GetRows(product, fields, props, context.Message.SearchDto,catProps.Message.PropertyTypes.Count);
            _paginationService.Pagination(context.Message.SearchDto.PageSize, product.StockItemsDto.Count);
            table.Fields = fields;
            table.Rows = rows;
            await context.RespondAsync(new GetProductStocksTableQueryResponse(table));
        }
        private static List<RowData> GetRows(Model.ReadDbModel.ProductItemReadDbCollection product, List<FieldData> fields, List<PropertyInfo> props, SearchDto searchDto,double categoryPropertiesCount)
        {
            var propsSize= Math.Round(40/ categoryPropertiesCount, 2);
            var skip = (searchDto.PageNumber - 1) * searchDto.PageSize;
            var stocks = product.StockItemsDto.Skip(skip).Take(searchDto.PageSize).ToList();
            var rows = new List<RowData>();
            int i = 1;
            foreach (var item in stocks)
            {
                var rowItems = new List<RowDataValue>();
                foreach (var field in fields)
                {
                    switch (field.Key)
                    {
                        case "Color":
                            {
                                var color = product.PropertiesDto.Where(w => w.PropertyType == PropertyType.Color)
                                                      .SelectMany(s => s.PropertyItemDtos)
                                                      .Where(w => w.Id == item.PropItems.Where(pw => pw.PropertyType == PropertyType.Color).Select(s => s.PropertyId).FirstOrDefault())
                                                      .Select(s => new { s.Id, s.Name, s.Value })
                                                      .FirstOrDefault();
                                rowItems.Add(new RowDataValue
                                {
                                    FieldName = field.Key,
                                    Type = field.Type,
                                    Size=propsSize,
                                    TypeEnum = field.TypeEnum,
                                    Id = color?.Id.ToString() ?? "",
                                    Value = color?.Name ?? "",
                                    Color = color?.Value ?? ""
                                });
                            }
                            break;
                        case "Guarantee":
                            {
                                var guarantee = product.PropertiesDto.Where(w => w.PropertyType == PropertyType.Guarantee)
                                                      .SelectMany(s => s.PropertyItemDtos)
                                                      .Where(w => w.Id == item.PropItems.Where(pw => pw.PropertyType == PropertyType.Guarantee).Select(s => s.PropertyId).FirstOrDefault())
                                                      .Select(s => new { s.Id, s.Name, s.Value })
                                                      .FirstOrDefault();
                                rowItems.Add(new RowDataValue
                                {
                                    FieldName = field.Key,
                                    Type = field.Type,
                                    Size=propsSize,
                                    TypeEnum = field.TypeEnum,
                                    Id = guarantee?.Id.ToString() ?? "",
                                    Value = guarantee?.Name ?? "",
                                });
                            }
                            break;
                        case "Size":
                            {
                                var size = product.PropertiesDto.Where(w => w.PropertyType == PropertyType.Size)
                                                         .SelectMany(s => s.PropertyItemDtos)
                                                         .Where(w => w.Id == item.PropItems.Where(pw => pw.PropertyType == PropertyType.Size).Select(s => s.PropertyId).FirstOrDefault())
                                                         .Select(s => new { s.Id, s.Name, s.Value })
                                                         .FirstOrDefault();
                                rowItems.Add(new RowDataValue
                                {
                                    FieldName = field.Key,
                                    Type = field.Type,
                                    Size=propsSize,
                                    TypeEnum = field.TypeEnum,
                                    Id = size?.Id.ToString(),
                                    Value = size?.Name ?? ""
                                });
                            }
                            break;
                        case "Index":
                            rowItems.Add(new RowDataValue
                            {
                                FieldName = field.Key,
                                Type = field.Type,
                                TypeEnum = field.TypeEnum,
                                Value = i.ToString(),
                                Size = 10
                            });
                            break;
                        case "IsAvailable":
                            {
                                var valueStatus = (props[field.Index - 1].GetValue(item, null)?.ToString() ?? "").ToLower();
                                var (Name, Title, Color) = field.StatusColors.FirstOrDefault(d => d.Name.ToLower() == valueStatus);
                                rowItems.Add(new RowDataValue
                                {
                                    FieldName = field.Key,
                                    Type = field.Type,
                                    TypeEnum = field.TypeEnum,
                                    Value = Title,
                                    Color = Color,
                                    Size=20
                                });
                            }
                            break;
                        default:
                            {
                                rowItems.Add(new RowDataValue
                                {
                                    FieldName = field.Key,
                                    Type = field.Type,
                                    TypeEnum = field.TypeEnum,
                                    Size=15,
                                    Value = props[field.Index - 1].GetValue(item, null)?.ToString() ?? ""
                                });
                                break;
                            }
                    }
                }
                rows.Add(new RowData
                {
                    Id = item.StockId.ToString(),
                    Index = i,
                    Key = i.ToString(),
                    Items = rowItems
                });
                i++;
            }
            return rows;


        }

        private static List<FieldData> GetFields(Response<GetCategoryPropertiesQueryResponse> catProps)
        {
            var propsSize=Math.Round((double)40/ catProps.Message.PropertyTypes.Count,2);
            var fields = new List<FieldData>
            {
                new FieldData
                {
                    Index = 1,
                    Key = "Index",
                    Label = "#",
                    Order = 1,
                    Size=10,
                    Type = nameof(TableValueType.Text),
                    TypeEnum = TableValueType.Text
                }
            };
            //? I did this to make sure their order is always the same !!!
            var props = catProps.Message.PropertyTypes.Select(s => s.PropertyType).ToList();
            if (props.Contains(PropertyType.Color))
            {
                fields.Add(new FieldData
                {
                    Index = fields.LastOrDefault().Index + 1,
                    Key = "Color",
                    Label = "رنگ",
                    Order = 2,
                    Size=propsSize,
                    Type = nameof(TableValueType.Color),
                    TypeEnum = TableValueType.Color
                });
            }
            if (props.Contains(PropertyType.Guarantee))
            {
                fields.Add(new FieldData
                {
                    Index = fields.LastOrDefault().Index + 1,
                    Key = "Guarantee",
                    Label = "گارانتی",
                    Order = 3,
                    Size=propsSize,
                    Type = nameof(TableValueType.Text),
                    TypeEnum = TableValueType.Text
                });
            }
            if (props.Contains(PropertyType.Size))
            {
                fields.Add(new FieldData
                {
                    Index = fields.LastOrDefault().Index + 1,
                    Key = "Size",
                    Label = "سایز",
                    Order = 4,
                    Size=propsSize,
                    Type = nameof(TableValueType.Text),
                    TypeEnum = TableValueType.Text
                });
            }
            #region comment
            // foreach (var item in catProps.Message.PropertyTypes)
            // {
            //     switch (item.PropertyType)
            //     {
            //         case PropertyType.Color:
            //             fields.Add(new FieldData
            //             {
            //                 Index = fields.LastOrDefault().Index + 1,
            //                 Key = "Color",
            //                 Label = "رنگ",
            //                 Order = 2,
            //                 Type = TableValueType.Color.ToString(),
            //                 TypeEnum = TableValueType.Color
            //             });

            //             break;
            //         case PropertyType.Guarantee:
            //             fields.Add(new FieldData
            //             {
            //                 Index = fields.LastOrDefault().Index + 1,
            //                 Key = "Guarantee",
            //                 Label = "گارانتی",
            //                 Order = 3,
            //                 Type = TableValueType.Text.ToString(),
            //                 TypeEnum = TableValueType.Text
            //             });

            //             break;
            //         case PropertyType.Size:
            //             fields.Add(new FieldData
            //             {
            //                 Index = fields.LastOrDefault().Index + 1,
            //                 Key = "Size",
            //                 Label = "سایز",
            //                 Order = 4,
            //                 Type = TableValueType.Text.ToString(),
            //                 TypeEnum = TableValueType.Text
            //             });
            //             break;
            //     }
            #endregion
            fields.Add(new FieldData
            {
                Index = 2,
                Key = "Count",
                Size = 15,
                Label = "موجودی",
                Order = fields.LastOrDefault().Index + 1,
                Type = nameof(TableValueType.Text),
                TypeEnum = TableValueType.Text
            });
            fields.Add(new FieldData
            {
                Index = 5,
                Key = "Price",
                Label = "قیمت فروش",
                Size=15,
                Order = fields.LastOrDefault().Index + 1,
                Type = nameof(TableValueType.Price),
                TypeEnum = TableValueType.Price
            });
            fields.Add(new FieldData
            {
                Index = 8,
                Key = "IsAvailable",
                Label = "وضعیت",
                Order = fields.LastOrDefault().Index + 1,
                Type = nameof(TableValueType.Status),
                TypeEnum = TableValueType.Status,
                Size=20,
                StatusColors = new List<(string Name, string Title, string Color)>() { ("true", "فعال", "rgb(52,195,143)"), ("false", "غیر فعال", "rgb(244,106,106)") }
            });
            return fields;
        }
    }
    public class GetProductStocksTableQueryResponse
    {
        public TableData TableData { get; set; }

        public GetProductStocksTableQueryResponse(TableData tableData)
        {
            TableData = tableData;
        }
    }
}