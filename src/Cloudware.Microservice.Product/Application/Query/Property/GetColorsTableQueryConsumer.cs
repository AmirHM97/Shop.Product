using System.Threading.Tasks;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using System.Linq;
using System.Collections.Generic;
using Cloudware.Utilities.Table;
using System.Text;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public class GetColorsTableQueryConsumer : IConsumer<GetColorsTableQuery>, IMediatorConsumerType
    {
        private readonly DbSet<Color> _colors;
        private readonly IUnitOfWork _unitOfWork;

        private readonly ITableDataManager _dataManager;
        public GetColorsTableQueryConsumer(IUnitOfWork unitOfWork, ITableDataManager dataManager)
        {
            _unitOfWork = unitOfWork;
            _colors = _unitOfWork.Set<Color>();
            _dataManager = dataManager;
        }
        public async Task Consume(ConsumeContext<GetColorsTableQuery> context)
        {
            // var name = "colorName";
            // var code = " code";
            var name = "عنوان رنگ";
            var code = " رنگ";

            var colors = _colors.Where( w => !w.IsDeleted && w.TenantId == context.Message.TenantId).AsQueryable();
            var mapField =
               new List<MapField>
               {
                new MapField{Name="Name",Title=name,Order=3,Type=TableValueType.Text,Size=25},
                new MapField{Name="Code",Title=code,Order=4,Type=TableValueType.Color,Size=50},
               };
               context.Message.SearchDto.SortBy="LastUpdatedDate";
            var res = await _dataManager.InitialTable(colors, context.Message.SearchDto, mapField, "Guid", null, true);
            await context.RespondAsync(new GetColorsTableQueryResponse(res));
        }
    }
    public class GetColorsTableQueryResponse
    {
        public TableData TableData { get; set; }

        public GetColorsTableQueryResponse(TableData tableData)
        {
            TableData = tableData;
        }
    }
}