using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Search_Filters
{
    public class GetAllSearchFiltersQuery: IRequestType
    {
        public string TenantId { get; set; }
        public GetAllSearchFiltersQuery(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}
