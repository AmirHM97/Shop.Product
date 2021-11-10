using System;
using System.Collections.Generic;

namespace Cloudware.Microservice.Product.Application.Command.Stock
{
    public record EditStockCommand(string TenantId,string ProductGuid,List<StockItemEditCommandDto>? StocksItems);
}