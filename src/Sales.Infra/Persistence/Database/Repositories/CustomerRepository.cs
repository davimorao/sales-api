using Sales.Domain.Entities;
using Sales.Domain.Repositories;
using System.Data;

namespace Sales.Infra.Persistence.Database.Repositories
{
    public class CustomerRepository(IDbConnection dbConnection) : GenericRepository<Customer>(dbConnection), ICustomerRepository
    {
    }
}
