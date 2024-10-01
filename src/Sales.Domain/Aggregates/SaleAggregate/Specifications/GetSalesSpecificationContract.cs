using Sales.Domain.Enums;
using Sales.Domain.Repositories;

namespace Sales.Domain.Aggregates.SaleAggregate;

public sealed class GetSalesSpecificationContract
{
    public long? Id { get; set; }
    public long? CustomerId { get; set; }
    public long? BranchId { get; set; }
    public DateTime? SaleDateFrom { get; set; }
    public DateTime? SaleDateTo { get; set; }
    public ESaleStatus? SaleStatus { get; set; }
    public decimal? MinTotalSaleValue { get; set; }
    public decimal? MaxTotalSaleValue { get; set; }

    public int? Skip { get; set; }
    public int? Take { get; set; }

    public ICollection<OrderingField>? OrderingFields { get; set; }
}
