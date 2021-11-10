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
    public class GetSizesQueryConsumer : IConsumer<GetSizesQuery>, IMediatorConsumerType
    {
        private readonly DbSet<Size> _Sizes;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaginationService _paginationService;


        public GetSizesQueryConsumer(IUnitOfWork unitOfWork, IPaginationService paginationService)
        {
            _unitOfWork = unitOfWork;
            _Sizes = _unitOfWork.Set<Size>();
            _paginationService = paginationService;
        }
        public async Task Consume(ConsumeContext<GetSizesQuery> context)
        {
            int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;
            var Sizes = _Sizes.Where(w=>!w.IsDeleted&&w.TenantId==context.Message.TenantId&&w.Name.Contains(context.Message.Query)).AsNoTracking().OrderByDescending(o=>o.LastUpdatedDate).Select(s => new GetSizesQueryResponseItem
            {
                Id = s.Id.ToString(),
                Name = s.Name
            });
            _paginationService.Pagination(Sizes, context.Message.PageSize);

            await context.RespondAsync(new GetSizesQueryResponse(await Sizes.Skip(skip).Take(context.Message.PageSize).ToListAsync()));
        }
    }
    public class GetSizesQueryResponse
    {
        public List<GetSizesQueryResponseItem> Sizes { get; set; }

        public GetSizesQueryResponse(List<GetSizesQueryResponseItem> sizes)
        {
            Sizes = sizes;
        }
    }
    public class GetSizesQueryResponseItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}