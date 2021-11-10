using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query
{
    public class GetAllProductCategoriesQuery:IRequestType
    {
        public string TenantId { get; set; }
        public GetAllProductCategoriesQuery(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}
