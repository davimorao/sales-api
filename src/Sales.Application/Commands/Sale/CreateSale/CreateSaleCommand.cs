using MediatR;
using Sales.Domain.Aggregates.SaleAggregate;

namespace Sales.Application.Commands
{
    public sealed class CreateSaleCommand : IRequest<BaseResponse<Sale>>
    {
        public required DateTime SaleDate { get; set; }
        public required long CustomerId { get; set; }
        public required long BranchId { get; set; }
        public required List<CreateSaleItemDto> Items { get; set; }
    }

    public class CreateSaleItemDto
    {
        public required long ProductId { get; set; }
        public required int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
