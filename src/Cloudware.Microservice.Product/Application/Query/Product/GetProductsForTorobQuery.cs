using Cloudware.Microservice.Product.DTO;
using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Product
{
    public class GetProductsForTorobQuery:IRequestType
    {
        public GetProductsForTorobQuery(GetProductsForTorobDto getProductsForTorobDto, string tenantId)
        {
            GetProductsForTorobDto = getProductsForTorobDto;
            TenantId = tenantId;
        }

        public GetProductsForTorobQuery(string tenantId, int page, int? pageSize)
        {
            TenantId = tenantId;
            //Page = page;
            //PageSize = pageSize;
        }

        public GetProductsForTorobDto GetProductsForTorobDto { get; set; }
        public string TenantId { get; set; }
        //public int Page { get; set; } 
        //public int? PageSize { get; set; } 
    }
}
