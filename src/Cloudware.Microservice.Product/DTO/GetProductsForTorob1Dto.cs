using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.DTO
{
    public class GetProductsForTorobDto
    {
        public string Page_Unique { get; set; }
        public string Page_Url { get; set; }
        public int Page { get; set; } = 1;
        public int? PageSize { get; set; } = 100;
        
    }
}
