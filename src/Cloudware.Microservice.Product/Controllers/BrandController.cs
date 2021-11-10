using Cloudware.Microservice.Product.Application.Command;
using Cloudware.Microservice.Product.Application.Query;
using Cloudware.Microservice.Product.Application.Query.Brand;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Utilities.Configure.Microservice;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cloudware.Microservice.Product.Application.Query.GetAllProductBrandsQueryConsumer;

namespace Cloudware.Microservice.Product.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BrandController: ClwControllerBase
    {
        private readonly IMediator _mediator;

        public BrandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand(CreateBrandDto createBrandDto)
        {
            var req = _mediator.CreateRequestClient<CreateBrandCommand>();
            var res = await req.GetResponse<CreateBrandCommandResponse>(new CreateBrandCommand(createBrandDto.Name, TenantId, createBrandDto.Icon));
            return Ok(res.Message);
        }
        [HttpGet]
        public async Task<IActionResult> GetBrands(int pageNumber=1,int pageSize=10)
        {
            var req = _mediator.CreateRequestClient<GetAllProductBrandsQuery>();
            var res = await req.GetResponse<GetAllProductBrandsQueryResponse>(new GetAllProductBrandsQuery(TenantId, pageSize,pageNumber));
            return Ok(res.Message);
        }  
        [HttpGet]
        public async Task<IActionResult> GetBrandById(long id)
        {
            var req = _mediator.CreateRequestClient<GetBrandByIdQuery>();
            var res = await req.GetResponse<GetBrandByIdQueryResponse>(new GetBrandByIdQuery(id,TenantId));
            return Ok(res.Message);
        }
    }
}
