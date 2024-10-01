using Dapper;
using Dapper.Contrib.Extensions;
using Sales.Domain.Repositories;
using System.Data;

namespace Sales.Infra.Persistence.Database.Repositories;

public class GenericRepository<T>(IDbConnection dbConnection) : IRepository<T> where T : class
{
    private readonly IDbConnection _dbConnection = dbConnection;

    public virtual async Task<IEnumerable<T>> GetBySpecificationAsync(ISpecification<T> specification)
    {
        var sql = specification.ToSqlQuery();
        var parameters = specification.Parameters;

        return await _dbConnection.QueryAsync<T>(sql, parameters);
    }

    public virtual async Task<T> GetByIdAsync(long id) => await _dbConnection.GetAsync<T>(id);

    public virtual async Task<long> InsertAsync(T entity) => await _dbConnection.InsertAsync(entity);

    public virtual async Task<bool> UpdateAsync(T entity) => await _dbConnection.UpdateAsync(entity);

    public virtual async Task<bool> DeleteAsync(T entity) => await _dbConnection.DeleteAsync(entity);

    public virtual async Task<bool> DeleteByIdAsync(long id)
    {
        var entity = await GetByIdAsync(id);

        if (entity is null)
            return false;

        return await DeleteAsync(entity);
    }
}