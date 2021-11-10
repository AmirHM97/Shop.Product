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
    public class GetGuaranteesTableQueryConsumer : IConsumer<GetGuaranteesTableQuery>, IMediatorConsumerType
    {
        private readonly DbSet<Guarantee> _guarantees;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITableDataManager _dataManager;
        public GetGuaranteesTableQueryConsumer(IUnitOfWork unitOfWork, ITableDataManager dataManager)
        {
            _unitOfWork = unitOfWork;
            _guarantees = _unitOfWork.Set<Guarantee>();
            _dataManager = dataManager;
        }
        public async Task Consume(ConsumeContext<GetGuaranteesTableQuery> context)
        {
            var guarantees = _guarantees.Where(w=>!w.IsDeleted&&w.TenantId==context.Message.TenantId).AsQueryable();
            var mapField =
               new List<MapField>
               {
                new MapField{Name="Name",Title="عنوان گارانتی",Order=3,Type=TableValueType.Text,Size=20},
                new MapField{Name="FrontImage",Title="عکس رو",Order=4,Type=TableValueType.Image,Size=25},
                new MapField{Name="BackImage",Title="عکس پشت",Order=4,Type=TableValueType.Image,Size=25},
                new MapField{Name="Duration",Title="مدت زمان گارانتی",Order=4,Type=TableValueType.Text,Size=20},
               };
                 context.Message.SearchDto.SortBy="LastUpdatedDate";
            var res = await _dataManager.InitialTable(guarantees, context.Message.SearchDto, mapField, "Guid", null);
            await context.RespondAsync(new GetGuaranteesTableQueryResponse(res));
        }
    }
    public class GetGuaranteesTableQueryResponse
    {
        public TableData TableData { get; set; }

        public GetGuaranteesTableQueryResponse(TableData tableData)
        {
            TableData = tableData;
        }
    }
}