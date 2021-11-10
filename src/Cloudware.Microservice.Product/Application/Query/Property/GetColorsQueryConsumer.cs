using System.Threading.Tasks;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using System.Linq;
using System.Collections.Generic;
using Cloudware.Utilities.Table;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public class GetColorsQueryConsumer : IConsumer<GetColorsQuery>, IMediatorConsumerType
    {
        private readonly DbSet<Color> _colors;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaginationService _paginationService;

        public GetColorsQueryConsumer(IUnitOfWork unitOfWork, IPaginationService paginationService)
        {
            _unitOfWork = unitOfWork;
            _colors = _unitOfWork.Set<Color>();
            _paginationService = paginationService;
        }

        public async Task Consume(ConsumeContext<GetColorsQuery> context)
        {
            int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;
            var colors = _colors.Where(w => !w.IsDeleted && w.TenantId == context.Message.TenantId &&w.Name.Contains(context.Message.Query)).AsNoTracking().OrderByDescending(o=>o.LastUpdatedDate).Select(s => new GetColorsQueryResponseItem
            {
                Code = s.Code,
                Id = s.Id.ToString(),
                Name = s.Name
            });
            _paginationService.Pagination(colors, context.Message.PageSize);
            await context.RespondAsync(new GetColorsQueryResponse(await colors.Skip(skip).Take(context.Message.PageSize).ToListAsync()));
        }
    }
    public class GetColorsQueryResponse
    {
        public List<GetColorsQueryResponseItem> Colors { get; set; }

        public GetColorsQueryResponse(List<GetColorsQueryResponseItem> colors)
        {
            Colors = colors;
        }
    }
    public class GetColorsQueryResponseItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}