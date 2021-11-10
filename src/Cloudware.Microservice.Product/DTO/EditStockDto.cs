using System.Collections.Generic;
using Cloudware.Microservice.Product.Application.Command;

namespace Cloudware.Microservice.Product.DTO
{
    public class EditStockDto
    {
        public string Guid { get; set; }
        public List<StockItemEditCommandDto>? StocksItems { get; set; }
    }
}