namespace Sales.API.Controllers.Responses
{
    public class SaleItemResponse
    {
        public long Id { get; set; }
        public long SaleId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalItemValue { get; set; }
    }
}
