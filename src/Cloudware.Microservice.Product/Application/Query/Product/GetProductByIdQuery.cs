using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query
{

    public record GetProductByIdQuery(long ProductItemId, string TenantId,int RelatedProductsSize = 10) : IRequestType;


}
