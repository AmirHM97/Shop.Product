using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public record GetPropertyQuery(string TenantId,PropertyType  PropertyType,int PageNumber,int PageSize):IRequestType;
  
}
