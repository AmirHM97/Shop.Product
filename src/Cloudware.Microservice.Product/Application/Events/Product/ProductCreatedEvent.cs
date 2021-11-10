using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Command;
using Cloudware.Microservice.Product.Model;
namespace Cloudware.Microservice.Product.Application.Events
{
    public class ProductCreatedEvent
    {
        public ProductItem  ProductItem { get; set; }
        public CreateProductCommand CreateProductCommand { get; set; }

        public ProductCreatedEvent(  ProductItem productItem,CreateProductCommand createProductCommand)
        {
            CreateProductCommand = createProductCommand;
            ProductItem = productItem;
        }



    }
}
