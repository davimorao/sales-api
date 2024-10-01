using Sales.Domain.Repositories;

namespace Sales.Domain.Aggregates.SaleAggregate
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetBySpecificationWithRelationShipAsync(ISpecification<Sale> specification);
    }
}
