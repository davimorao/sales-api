using MediatR;
using Sales.Domain.Entities;

namespace Sales.Application.Commands
{
    public class UpdateProductCommand : IRequest<BaseResponse<Product>>
    {
        public long Id { get; set; }
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
