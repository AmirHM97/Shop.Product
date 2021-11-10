using Cloudware.Utilities.Contract.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Cloudware.Microservice.Product.Model.ReadDbModel
{
    public class PropertyReadDb
    {
        public long PropertyId { get; set; }
        public string Name { get; set; }//color guarantee
        public PropertyType PropertyType { get; set; }
        public PropertyViewType PropertyViewType { get; set; }
        public List<PropertyItemReadDb> PropertyItemDtos { get; set; }
    }
    public class PropertyItemReadDb
    {
        public long Id { get; set; }
        public string Name { get; set; }// red guarantee name
        public string? Value { get; set; }
    }
    /// <summary>
    /// 0 = Button | 1 = RadioButton | 2 = Color
    /// </summary>

}
