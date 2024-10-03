using Sales.Domain.Entities;
using System.Data;

namespace Sales.Infra.Persistence.Repositories
{
    public sealed class CustomerRepository(IDbConnection dbConnection) : SqlRepository<Customer>(dbConnection), ICustomerRepository
    {
    }
}
