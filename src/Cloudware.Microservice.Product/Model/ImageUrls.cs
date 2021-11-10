using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Model
{
    public class ImageUrlItems
    {
        public ImageUrlItems(string url, long productItemId)
        {
            Url = url;
            ProductItemId = productItemId;
        }
        public ImageUrlItems()
        {

        }
        public long Id { get; set; }
        public string Url { get; set; }
        public long ProductItemId { get; set; }
        public bool IsDeleted { get; set; }

        public ProductItem ProductItem { get; set; }
    }
}
