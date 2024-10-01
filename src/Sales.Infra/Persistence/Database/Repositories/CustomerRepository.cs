using Sales.Domain.Entities;
using System.Data;

namespace Sales.Infra.Persistence.Database.Repositories
{
    public sealed class CustomerRepository(IDbConnection dbConnection) : GenericRepository<Customer>(dbConnection), ICustomerRepository
    {
    }
}
