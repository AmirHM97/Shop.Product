using System;
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
    public class GetGuaranteesQueryConsumer : IConsumer<GetGuaranteesQuery>, IMediatorConsumerType
    {
        private readonly DbSet<Guarantee> _guarantees;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaginationService _paginationService;

        public GetGuaranteesQueryConsumer(IUnitOfWork unitOfWork, IPaginationService paginationService)
        {
            _unitOfWork = unitOfWork;
            _guarantees = _unitOfWork.Set<Guarantee>();
            _paginationService = paginationService;
        }

        public async Task Consume(ConsumeContext<GetGuaranteesQuery> context)
        {
            int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;
            var guarantees = _guarantees.Where(w => !w.IsDeleted && w.TenantId == context.Message.TenantId&&w.Name.Contains(context.Message.Query)).AsNoTracking().OrderByDescending(o=>o.LastUpdatedDate).Select(s => new GetGuaranteesQueryResponseItem
            {
                BackImage = s.BackImage,
                FrontImage = s.FrontImage,
                Duration = s.Duration,
                Id = s.Id.ToString(),
                Name = s.Name
            });
            _paginationService.Pagination(guarantees, context.Message.PageSize);

            await context.RespondAsync(new GetGuaranteesQueryResponse(await guarantees.Skip(skip).Take(context.Message.PageSize).ToListAsync()));
        }
    }
    public class GetGuaranteesQueryResponse
    {
        public List<GetGuaranteesQueryResponseItem> Guarantees { get; set; }

        public GetGuaranteesQueryResponse(List<GetGuaranteesQueryResponseItem> guarantees)
        {
            Guarantees = guarantees;
        }
    }
    public class GetGuaranteesQueryResponseItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public string BackImage { get; set; }
        public string FrontImage { get; set; }
    }
}