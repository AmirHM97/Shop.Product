using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public class GetColorByIdQueryConsumer : IConsumer<GetColorByIdQuery>, IMediatorConsumerType
    {
        private readonly DbSet<Color> _colors;
        private readonly IUnitOfWork _unitOfWork;
        public GetColorByIdQueryConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _colors = _unitOfWork.Set<Color>();
        }
        public async Task Consume(ConsumeContext<GetColorByIdQuery> context)
        {
            var color = await _colors.FirstOrDefaultAsync(f =>!f.IsDeleted&& f.Guid.ToString().ToLower() == context.Message.Guid.ToLower()&&f.TenantId==context.Message.TenantId);
            await context.RespondAsync(new GetColorByIdQueryResponse(color.Id,color.Name,color.Code,color.Guid.ToString()));
        }
    }
    public class GetColorByIdQueryResponse
    {
        public GetColorByIdQueryResponse(long id, string name, string code, string guid)
        {
            Id = id;
            Name = name;
            Code = code;
            Guid = guid;
        }

        public long Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}