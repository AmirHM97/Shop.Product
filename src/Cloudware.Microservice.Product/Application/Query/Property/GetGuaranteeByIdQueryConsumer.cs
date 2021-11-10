using System;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public class GetGuaranteeByIdQueryConsumer : IConsumer<GetGuaranteeByIdQuery>, IMediatorConsumerType
    {
        private readonly DbSet<Guarantee> _guarantees;
        private readonly IUnitOfWork _unitOfWork;
        public GetGuaranteeByIdQueryConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _guarantees = _unitOfWork.Set<Guarantee>();
        }
        public async Task Consume(ConsumeContext<GetGuaranteeByIdQuery> context)
        {
            var guarantee = await _guarantees.FirstOrDefaultAsync(f =>!f.IsDeleted&& f.Guid.ToString().ToLower() == context.Message.Guid.ToLower()&&f.TenantId==context.Message.TenantId);
            await context.RespondAsync(new GetGuaranteeByIdQueryResponse(guarantee.Id, guarantee.Name, guarantee.BackImage, guarantee.FrontImage, guarantee.Duration,guarantee.Guid.ToString()));
        }
    }
    public class GetGuaranteeByIdQueryResponse
    {
        public GetGuaranteeByIdQueryResponse(long id, string name, string backImage, string frontImage, int duration, string guid)
        {
            Id = id;
            Name = name;
            BackImage = backImage;
            FrontImage = frontImage;
            Duration = duration;
            Guid = guid;
        }

        public long Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string BackImage { get; set; }
        public string FrontImage { get; set; }
        public int Duration { get; set; }

    }
}