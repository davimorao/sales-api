using Sales.Domain.Entities;
using Sales.Domain.Entities.Products;
using System.Data;

namespace Sales.Infra.Persistence.Database.Repositories
{
    public sealed class ProductRepository(IDbConnection dbConnection) : GenericRepository<Product>(dbConnection), IProductRepository
    {
    }
}
