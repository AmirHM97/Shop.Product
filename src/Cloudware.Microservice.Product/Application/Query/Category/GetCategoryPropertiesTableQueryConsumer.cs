using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.DTO.Property;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Table;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public class GetCategoryPropertiesTableQueryConsumer : IConsumer<GetCategoryPropertiesTableQuery>, IMediatorConsumerType
    {
        private readonly DbSet<PropertyCategory> _propertyCategories;
        private readonly IUnitOfWork _uow;
        private readonly ITableDataManager _tableDataManager;

        public GetCategoryPropertiesTableQueryConsumer(IUnitOfWork uow, ITableDataManager tableDataManager)
        {
            _uow = uow;
            _propertyCategories = _uow.Set<PropertyCategory>();
            _tableDataManager = tableDataManager;
        }
        public async Task Consume(ConsumeContext<GetCategoryPropertiesTableQuery> context)
        {
            var query = _propertyCategories.Where(w => w.CategoryId == context.Message.CategoryId && w.TenantId == context.Message.TenantId).Select(s => new CategoryPropertyDto
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                Name = ProductExtensions.GetPropertyName(s.PropertyType),
                PropertyType = s.PropertyType,
            });
            var a = await query.ToListAsync();
            var mapField =
              new List<MapField>
              {
                new MapField{Name="Name",Title="عنوان موجودی",Order=1,Type=TableValueType.Text},
                 new MapField{Name="PropertyType",Title="لیست تنوع",Order=2,Type=TableValueType.Icon},
              };
            var res = await _tableDataManager.InitialTable(query, context.Message.SearchDto, mapField, "Id");
            await context.RespondAsync(new GetCategoryPropertiesTableQueryResponse(res));

        }
    }
    public class GetCategoryPropertiesTableQueryResponse
    {
        public TableData TableData { get; set; }

        public GetCategoryPropertiesTableQueryResponse(TableData tableData)
        {
            TableData = tableData;
        }
    }
}