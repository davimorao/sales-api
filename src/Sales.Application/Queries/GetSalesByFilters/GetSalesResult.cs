using Sales.Domain.Aggregates.SaleAggregate;

namespace Sales.Application.Queries.GetSalesByFilters;

public class GetSalesResult
{
    public IEnumerable<Sale> Sales { get; set; } = [];
}
