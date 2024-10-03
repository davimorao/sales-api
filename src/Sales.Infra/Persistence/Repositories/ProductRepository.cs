using Sales.Domain.Entities;
using Sales.Domain.Entities.Products;
using System.Data;

namespace Sales.Infra.Persistence.Repositories
{
    public sealed class ProductRepository(IDbConnection dbConnection) : SqlRepository<Product>(dbConnection), IProductRepository
    {
    }
}
