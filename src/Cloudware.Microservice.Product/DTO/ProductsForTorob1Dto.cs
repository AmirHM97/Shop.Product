using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.DTO
{
    public class ProductsForTorob1Dto
    {
        public List<productFortorob> productFortorob { get; set; }
    }

    public class productFortorob
    {
        public string Product_Id { get; set; }
        public string Page_Url { get; set; }
        public string Price { get; set; }
        public string Availability { get; set; }
        public string Old_Price { get; set; }
    }
}
