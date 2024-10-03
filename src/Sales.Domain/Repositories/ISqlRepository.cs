namespace Sales.Domain.Repositories;

public interface ISqlRepository<T>
{
    Task<IEnumerable<T>> GetBySpecificationAsync(ISqlSpecification<T> specification);
    Task<bool> DeleteAsync(T entity);
    Task<bool> DeleteByIdAsync(long id);
    Task<T> GetByIdAsync(long id);
    Task<long> InsertAsync(T entity);
    Task<bool> UpdateAsync(T entity);
}
