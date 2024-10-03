namespace Sales.Domain.Entities.Products.Events
{
    public class ProductCreatedEvent
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal Price { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public ProductCreatedEvent(Guid productId, string productName, decimal price, DateTime createdAt)
        {
            ProductId = productId;
            ProductName = productName;
            Price = price;
            CreatedAt = createdAt;
        }
    }
}