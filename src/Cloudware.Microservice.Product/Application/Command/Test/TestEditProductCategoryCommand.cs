using System.Collections.Generic;

namespace Cloudware.Microservice.Product.Application.Command.Test
{
    public record TestEditProductCategoryCommand(List<long> Ids,long CategoryId);
   
}