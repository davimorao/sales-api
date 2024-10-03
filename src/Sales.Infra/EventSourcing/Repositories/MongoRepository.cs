using MongoDB.Bson;
using MongoDB.Driver;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;

namespace Sales.Infra.EventSourcing.Repositories
{
    public class MongoRepository : IMongoRepository
    {
        private readonly IMongoCollection<DomainEvent> _collection;

        public MongoRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<DomainEvent>("Events");
        }

        public async Task InsertAsync(DomainEvent domainEvent)
        {
            await _collection.InsertOneAsync(domainEvent);
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsByAggregateTypeAsync(string aggregateType)
        {
            var filter = Builders<DomainEvent>.Filter.Eq(es => es.AggregateType, aggregateType);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<DomainEvent> GetEventByIdAsync(string id)
        {
            var filter = Builders<DomainEvent>.Filter.Eq(es => es.Id, ObjectId.Parse(id));
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public T DeserializeEventData<T>(DomainEvent domainEvent)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(domainEvent.EventData);
        }
    }
}
