namespace Sales.Domain.Entities.Products.Events
{
    public class ProductUpdatedEvent
    {
        public Guid ProductId { get; private set; }
        public string UpdatedProductName { get; private set; }
        public decimal UpdatedPrice { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public ProductUpdatedEvent(Guid productId, string updatedProductName, decimal updatedPrice, DateTime updatedAt)
        {
            ProductId = productId;
            UpdatedProductName = updatedProductName;
            UpdatedPrice = updatedPrice;
            UpdatedAt = updatedAt;
        }
    }
}