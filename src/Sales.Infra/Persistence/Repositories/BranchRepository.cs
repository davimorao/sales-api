using Sales.Domain.Entities;
using System.Data;

namespace Sales.Infra.Persistence.Repositories
{
    public sealed class BranchRepository(IDbConnection dbConnection) : SqlRepository<Branch>(dbConnection), IBranchRepository
    {
    }
}
