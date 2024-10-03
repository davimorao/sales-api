using Sales.Domain.Entities;

namespace Sales.Domain.Repositories
{
    public interface IMongoRepository
    {
        Task InsertAsync(DomainEvent domainEvent);
        Task<IEnumerable<DomainEvent>> GetEventsByAggregateTypeAsync(string aggregateType);
        Task<DomainEvent> GetEventByIdAsync(string id);
        T DeserializeEventData<T>(DomainEvent domainEvent);
    }
}
