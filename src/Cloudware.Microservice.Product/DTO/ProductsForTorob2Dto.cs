using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.DTO
{
    public class ProductsForTorob2Dto
    {
        public string Count { get; set; }
        public string Max_Pages { get; set; }
        public List<Products>  Products { get; set; }

    }

    public class Products
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Page_Unique { get; set; }
        public string Current_Price { get; set; }
        public string Old_Price { get; set; }
        public string Availability { get; set; }
        public string Category_Name { get; set; }
        public string Image_Link { get; set; }
        public string Page_Url { get; set; }
        public string Short_Dsc { get; set; }
        public Spec Spec { get; set; }
        public string Registry { get; set; }
        public string Guarantee { get; set; }
    }

    public class Spec
    {
        public string Memory { get; set; }
        public string Camera { get; set; }
        public string Color { get; set; }
    }
}
