using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Table;
using MassTransit;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Query.Product.Admin
{
    public class GetAdvancedProductTableQueryConsumer : IConsumer<GetAdvancedProductTableQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly ITableDataManager _dataManager;

        public GetAdvancedProductTableQueryConsumer(IProductReadDbContext productReadDbContext, ITableDataManager dataManager)
        {
            _productReadDbContext = productReadDbContext;
            _dataManager = dataManager;
        }

        public async Task Consume(ConsumeContext<GetAdvancedProductTableQuery> context)
        {
            var mapField =
           new List<MapField>
           {
                new MapField{Name="ImageUrl",Title="#",Order=1,Type=TableValueType.Image,Size=5},
                new MapField{Name="Code",Title="شناسه",Order=2,Type=TableValueType.Text},
                new MapField{Name="Name",Title="عنوان محصول",Order=3,Type=TableValueType.Text,Size=35},
                new MapField{Name="CategoryName",Title="دسته بندی",Order=4,Type=TableValueType.Text},
                new MapField{Name="TotalCount",Title="موجودی",Order=5,Type=TableValueType.Text},
                new MapField{Name="MinPrice",Title="کمترین قیمت",Order=6,Type=TableValueType.Price},
                new MapField{Name="MaxPrice",Title="بیشترین قیمت",Order=7,Type=TableValueType.Price},
                new MapField{Name="IsAvailable",Title="وضعیت",Order=8,Type=TableValueType.Status,StatusColors=new List<(string Name, string Title, string Color)>(){("true","فعال", "rgb(52,195,143)"), ("false", "غیر فعال", "rgb(244,106,106)") } },
           };
            IQueryable<ProductItemReadDbCollection> collection = _productReadDbContext.ProductItemsDataCollection.AsQueryable().Where(f => f.TenantId == context.Message.TenantId && !f.IsDeleted.Value);
                context.Message.SearchDto.SortBy = "LastUpdatedDate";

            if (!string.IsNullOrEmpty(context.Message.SearchDto.Search))
            {
                var search = context.Message.SearchDto.Search;
                collection = collection.Where(d =>
                      d.Name.Contains(search)
                   || d.Code.Contains(search)
                   || d.CategoryName.Contains(search)
                   );
                context.Message.SearchDto.Search = "";
            }

            var res = await _dataManager.InitialTable(collection, context.Message.SearchDto, mapField, "Guid");
            await context.RespondAsync(new GetAdvancedProductTableQueryResponse(res));
        }
    }
    public class GetAdvancedProductTableQueryResponse
    {
        public TableData TableData { get; set; }

        public GetAdvancedProductTableQueryResponse(TableData tableData)
        {
            TableData = tableData;
        }
    }
}