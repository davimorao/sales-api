using MongoDB.Bson;

namespace Sales.Domain.Entities
{
    public class DomainEvent
    {
        public DomainEvent()
        {
            Id = ObjectId.GenerateNewId();
            Timestamp = DateTime.UtcNow;
        }

        public ObjectId Id { get; private set; }
        public string? AggregateType { get; set; }
        public string? EventType { get; set; }
        public string? EventData { get; set; }
        public DateTime Timestamp { get; set; }
    }
}