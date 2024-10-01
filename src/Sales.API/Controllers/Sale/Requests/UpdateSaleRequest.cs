using Sales.Application.Commands;
using Sales.Domain.Enums;

namespace Sales.API.Controllers.Requests
{
    public class UpdateSaleRequest
    {
        public long Id { get; set; }
        public DateTime? SaleDate { get; set; }
        public long? CustomerId { get; set; }
        public long? BranchId { get; set; }
        public ESaleStatus? SaleStatus { get; set; }
        public IEnumerable<UpdateSaleItemRequest> Items { get; set; } = [];

        public UpdateSaleCommand ToCommand()
        {
            return new UpdateSaleCommand
            {
                Id = Id,
                SaleDate = SaleDate,
                CustomerId = CustomerId,
                BranchId = BranchId,
                SaleStatus = SaleStatus,
                Items = Items.Select(i => new UpdateSaleItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Discount = i.Discount
                }).ToList()
            };
        }
    }

    public class UpdateSaleItemRequest
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
