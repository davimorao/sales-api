using Sales.Domain.Repositories;

namespace Sales.Domain.Entities
{
    public interface ICustomerRepository : ISqlRepository<Customer>
    {
    }
}
