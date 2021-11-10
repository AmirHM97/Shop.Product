using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.HomePage
{
    public class GetHomePageDataQuery:IRequestType
    {
        public string TenantId { get; set; }
        public GetHomePageDataQuery(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}
