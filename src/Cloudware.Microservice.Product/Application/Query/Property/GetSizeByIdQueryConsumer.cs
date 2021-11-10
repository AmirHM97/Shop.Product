using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public class GetSizeByIdQueryConsumer : IConsumer<GetSizeByIdQuery>, IMediatorConsumerType
    {
        private readonly DbSet<Size> _sizes;
        private readonly IUnitOfWork _unitOfWork;
        public GetSizeByIdQueryConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _sizes = _unitOfWork.Set<Size>();
        }
        public async Task Consume(ConsumeContext<GetSizeByIdQuery> context)
        {
            var size = await _sizes.FirstOrDefaultAsync(f =>!f.IsDeleted&&f.TenantId==context.Message.TenantId &&f.Guid.ToString().ToLower()== context.Message.Guid.ToLower());
            await context.RespondAsync(new GetSizeByIdQueryResponse(size.Id, size.Name,size.Guid.ToString()));
        }
    }
    public class GetSizeByIdQueryResponse
    {
        public GetSizeByIdQueryResponse(long id, string name, string guid)
        {
            Id = id;
            Name = name;
            Guid = guid;
        }

        public long Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }


    }
}