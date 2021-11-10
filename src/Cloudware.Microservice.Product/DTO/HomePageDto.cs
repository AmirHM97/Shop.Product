using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.DTO
{
    public class HomePageDto
    {
        //public List<ProductItemsListCatalogDto> NewestProducts { get; set; }
        public Section NewestProducts { get; set; }
        //public List<ProductItemsListCatalogDto> HottestProducts { get; set; }
        public Section HottestProducts { get; set; }
        //public List<ProductItemsListCatalogDto> SpecialOfferProducts { get; set; }
        public Section SpecialOfferProducts { get; set; }

        //public List<ProductItemsListCatalogDto> MostViewdProducts { get; set; }
        public Section MostViewedProducts { get; set; }

        //public List<ProductItemsListCatalogDto> WeeklySuggestedProducts { get; set; }
        public Section WeeklySuggestedProducts { get; set; }
        public Section HaveDiscountProducts { get; set; }

        //public List<ProductItemsListCatalogDto> FoodProducts { get; set; }
        public Section FoodProducts { get; set; }


    }
    public class Section 
    {
        public List<ProductItemsListCatalogDto> Products { get; set; }
        public string Link { get; set; }
    }
}
