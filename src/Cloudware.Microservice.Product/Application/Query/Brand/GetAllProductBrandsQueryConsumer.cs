using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query
{
    public class GetAllProductBrandsQueryConsumer : IConsumer<GetAllProductBrandsQuery>, IMediatorConsumerType
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductBrand> _productBrand;

        public GetAllProductBrandsQueryConsumer(IUnitOfWork uow)
        {
            _uow = uow;
            _productBrand = _uow.Set<ProductBrand>();
        }

        public async Task Consume(ConsumeContext<GetAllProductBrandsQuery> context)
        {
            var skip=(context.Message.PageNumber-1)*context.Message.PageSize;
            var brands = await _productBrand.Where(W=>W.TenantId==context.Message.TernantId).Skip(skip).Take(context.Message.PageSize).Select(s => new ProductBrandDto { Id = s.Id, Name = s.Name }).ToListAsync();
            await context.RespondAsync(new GetAllProductBrandsQueryResponse(brands));
        }

        //public async Task<List<ProductBrandDto>> Handle(GetAllProductBrandsQuery request, CancellationToken cancellationToken)
        //{
        //    return await _productBrand.Select(s => new ProductBrandDto {Id=s.Id,Name=s.Name }).ToListAsync();
        //}
        public class GetAllProductBrandsQueryResponse
        {
            public List<ProductBrandDto> ProductBrandsDto { get; set; }

            public GetAllProductBrandsQueryResponse(List<ProductBrandDto> productBrandsDto)
            {
                ProductBrandsDto = productBrandsDto;
            }
        }
    }
}
