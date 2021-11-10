using Cloudware.Microservice.Product.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Command
{
    public class ExportProductsToExcelCommand
    {
        public List<ProductItemsListCatalogDto> Products { get; set; }
    }
}
