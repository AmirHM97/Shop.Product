using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Sellers
{
    public class GetSellersQuery:IRequestType
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string TenantId { get; set; }
        public GetSellersQuery(int pageSize, int pageNumber, string tenantId)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            TenantId = tenantId;
        }
    }
}
