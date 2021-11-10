using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Table;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public class GetSizesTableQueryConsumer : IConsumer<GetSizesTableQuery>, IMediatorConsumerType
    {
        private readonly DbSet<Size> _Sizes;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITableDataManager _dataManager;

        public GetSizesTableQueryConsumer(IUnitOfWork unitOfWork, ITableDataManager dataManager)
        {
            _unitOfWork = unitOfWork;
            _Sizes = _unitOfWork.Set<Size>();
            _dataManager = dataManager;
        }
        public async Task Consume(ConsumeContext<GetSizesTableQuery> context)
        {
            var sizes = _Sizes.Where(w=>!w.IsDeleted&&w.TenantId==context.Message.TenantId).AsQueryable();
            var mapField =
               new List<MapField>
               {
                new MapField{Name="Name",Title="سایز",Order=3,Type=TableValueType.Text,Size=60},

               };
                 context.Message.SearchDto.SortBy="LastUpdatedDate";
            var res = await _dataManager.InitialTable(sizes, context.Message.SearchDto, mapField, "Guid", null, true);
            await context.RespondAsync(new GetSizesTableQueryResponse(res));
        }
    }
     public class GetSizesTableQueryResponse
    {
        public TableData TableData { get; set; }

        public GetSizesTableQueryResponse(TableData tableData)
        {
            TableData = tableData;
        }
    }
}