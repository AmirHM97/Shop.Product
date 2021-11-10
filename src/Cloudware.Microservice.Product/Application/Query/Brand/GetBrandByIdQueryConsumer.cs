using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Query.Brand
{
    public class GetBrandByIdQueryConsumer : IConsumer<GetBrandByIdQuery>, IMediatorConsumerType
    {
        private readonly DbSet<ProductBrand> _productBrands;
        private readonly IUnitOfWork _unitOfWork;

        public GetBrandByIdQueryConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _productBrands = _unitOfWork.Set<ProductBrand>();
        }

        public async Task Consume(ConsumeContext<GetBrandByIdQuery> context)
        {
            var brand = await _productBrands.FirstOrDefaultAsync(f => f.TenantId == context.Message.TenantId && f.Id == context.Message.Id);
            await context.RespondAsync(new GetBrandByIdQueryResponse(brand.Id, brand.Guid.ToString(), brand.Name, brand.Icon));
        }
    }
    public class GetBrandByIdQueryResponse
    {
        public GetBrandByIdQueryResponse(long id, string guid, string name, string icon)
        {
            Id = id;
            Guid = guid;
            Name = name;
            Icon = icon;
        }

        public long Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}