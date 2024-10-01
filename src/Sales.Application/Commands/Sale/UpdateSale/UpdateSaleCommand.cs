using MediatR;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Enums;

namespace Sales.Application.Commands
{
    public class UpdateSaleCommand : IRequest<BaseResponse<Sale>>
    {
        public long Id { get; set; }
        public DateTime? SaleDate { get; set; }
        public long? CustomerId { get; set; }
        public long? BranchId { get; set; }
        public ESaleStatus? SaleStatus { get; set; }
        public required List<UpdateSaleItemDto> Items { get; set; } = [];
    }

    public class UpdateSaleItemDto
    {
        public required long ProductId { get; set; }
        public required int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
