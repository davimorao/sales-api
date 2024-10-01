using MediatR;
using Sales.Domain.Entities;

namespace Sales.Application.Commands
{
    public class CreateProductCommand : IRequest<BaseResponse<Product>>
    {
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
