using Sales.Domain.Repositories;

namespace Sales.Domain.Entities;

public sealed class GetProductsSpecificationContract
{
    public long? Id { get; set; }
    public string? ProductName { get; set; }
    public decimal UnitPrice { get; set; }

    public int? Skip { get; set; }
    public int? Take { get; set; }

    public List<OrderingField>? OrderingFields { get; set; }
}
