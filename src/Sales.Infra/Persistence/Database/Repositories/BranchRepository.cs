using Sales.Domain.Entities;
using System.Data;

namespace Sales.Infra.Persistence.Database.Repositories
{
    public sealed class BranchRepository(IDbConnection dbConnection) : GenericRepository<Branch>(dbConnection), IBranchRepository
    {
    }
}
