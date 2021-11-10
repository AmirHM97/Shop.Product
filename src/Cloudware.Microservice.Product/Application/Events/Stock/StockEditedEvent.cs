namespace Cloudware.Microservice.Product.Application.Events.Stock
{
    public record StockEditedEvent(string TenantId,long ProductId);
    
}