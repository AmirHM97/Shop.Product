using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Command
{
    public class CreateBrandCommandConsumer : IConsumer<CreateBrandCommand>, IMediatorConsumerType
    {
        private readonly DbSet<ProductBrand> _brands;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBrandCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _brands = _unitOfWork.Set<ProductBrand>();
        }

        public async Task Consume(ConsumeContext<CreateBrandCommand> context)
        {
            var brand = new ProductBrand
            {
                IsDeleted= false,
                Name=context.Message.Name,
                TenantId=context.Message.TenantId,
                Icon=context.Message.Icon
            };
            await _brands.AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();
            await context.RespondAsync(new CreateBrandCommandResponse(brand.Id));
        }

    }
    public class CreateBrandCommandResponse
    {
        public long BrandId { get; set; }

        public CreateBrandCommandResponse(long brandId)
        {
            BrandId = brandId;
        }
    }
}
