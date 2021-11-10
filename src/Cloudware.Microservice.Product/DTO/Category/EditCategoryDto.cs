namespace Cloudware.Microservice.Product.DTO.Category
{
    public class EditCategoryDto
    {
        public string Guid { get; set; }
        public long ParentId { get; set; }
        public string?  Name { get; set; }
        public string?  Description { get; set; }
        public string?  Icon { get; set; }
    }
}