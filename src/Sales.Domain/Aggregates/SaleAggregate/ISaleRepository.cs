using Sales.Domain.Repositories;

namespace Sales.Domain.Aggregates.SaleAggregate
{
    public interface ISaleRepository : ISqlRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetBySpecificationWithRelationShipAsync(ISqlSpecification<Sale> specification);
    }
}
