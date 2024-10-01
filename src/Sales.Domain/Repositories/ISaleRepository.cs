using Sales.Domain.Aggregates.SaleAggregate;

namespace Sales.Domain.Repositories
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetBySpecificationWithRelationShipAsync(ISpecification<Sale> specification);
    }
}
