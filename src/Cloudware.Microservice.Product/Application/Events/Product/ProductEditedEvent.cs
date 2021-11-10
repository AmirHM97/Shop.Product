using Cloudware.Microservice.Product.Application.Command;
using Cloudware.Microservice.Product.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events
{
    public class ProductEditedEvent
    {
        public long ProductId { get; set; }
        public EditProductCommand EditProductCommand { get; set; }
        public ProductItem ProductItem { get; set; }

        public ProductEditedEvent(EditProductCommand editProductCommand, ProductItem productItem, long productId)
        {
            EditProductCommand = editProductCommand;
            ProductItem = productItem;
            ProductId = productId;
        }

        // public ProductItem ProductItem { get; set; }

        //public ProductEditedEvent(ProductItem productItem)
        //{
        //    ProductItem = productItem;
        //}
    }
}
