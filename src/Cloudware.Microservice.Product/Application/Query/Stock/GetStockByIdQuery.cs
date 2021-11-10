using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Stock
{
    public record GetStockByIdQuery(long Id,long ProductId):IRequestType;

}
