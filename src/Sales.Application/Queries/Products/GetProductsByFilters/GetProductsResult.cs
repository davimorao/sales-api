using Sales.Domain.Entities;

namespace Sales.Application.Queries.GetProductsByFilters;

public class GetProductsResult
{
    public IEnumerable<Product> Products { get; set; } = [];
}
