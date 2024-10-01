using Sales.Application.Queries.GetProductsByFilters;
using Sales.Domain.Entities;

namespace Sales.API.Controllers.Sale.Requests
{
    public class GetProductRequest
    {
        public long? Id { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }

        public int? Skip { get; set; }
        public int? Take { get; set; }

        public GetProductsQuery ToCommand() => new(new GetProductsSpecificationContract
        {
            Id = Id,
            ProductName = ProductName,
            UnitPrice = UnitPrice,
            Skip = Skip,
            Take = Take
        });
    }
}
