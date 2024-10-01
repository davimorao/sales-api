using Sales.Application.Commands;

namespace Sales.API.Controllers.Requests
{
    public class CreateSaleRequest
    {
        public DateTime SaleDate { get; set; }
        public long CustomerId { get; set; }
        public long BranchId { get; set; }
        public List<CreateSaleItemRequest> Items { get; set; }

        public CreateSaleCommand ToCommand()
        {
            return new CreateSaleCommand
            {
                SaleDate = SaleDate,
                CustomerId = CustomerId,
                BranchId = BranchId,
                Items = Items.Select(i => new CreateSaleItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Discount = i.Discount
                }).ToList()
            };
        }
    }

    public class CreateSaleItemRequest
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
