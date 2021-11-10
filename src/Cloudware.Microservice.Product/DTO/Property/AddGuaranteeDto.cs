using System;

namespace Cloudware.Microservice.Product.DTO.Property
{
    public class AddGuaranteeDto
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public string FrontImage { get; set; }
        public string BackImage { get; set; }

    }
}