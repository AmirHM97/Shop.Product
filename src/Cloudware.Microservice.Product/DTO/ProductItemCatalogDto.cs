using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.DTO
{
    public class ProductItemsListCatalogDto
    {
        public long Id { get; set; }
        public string image { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
    }
}
