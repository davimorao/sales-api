using Sales.Domain.Repositories;

namespace Sales.Domain.Entities.Products
{
    public interface IProductRepository : ISqlRepository<Product>
    {
    }
}
