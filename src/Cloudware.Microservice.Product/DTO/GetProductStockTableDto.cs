namespace Cloudware.Microservice.Product.DTO
{
    public class GetProductStockTableDto
    {
        public Utilities.Table.SearchDto SearchDto { get; set; }
        public long ProductId { get; set; }
    }
}