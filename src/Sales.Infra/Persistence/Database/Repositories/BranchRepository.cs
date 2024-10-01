using Sales.Domain.Entities;
using Sales.Domain.Repositories;
using System.Data;

namespace Sales.Infra.Persistence.Database.Repositories
{
    public class BranchRepository(IDbConnection dbConnection) : GenericRepository<Branch>(dbConnection), IBranchRepository
    {
    }
}
