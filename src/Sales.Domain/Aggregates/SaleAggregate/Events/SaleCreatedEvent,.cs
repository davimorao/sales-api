using Sales.Domain.Entities;
using System.Text.Json;

namespace Sales.Domain.Aggregates.SaleAggregate.Events
{
    public class SaleCreatedEvent : DomainEvent
    {
        public SaleCreatedEvent() { }
        public SaleCreatedEvent(Sale sale)
        {
            AggregateType = "Sale";
            EventType = "SaleCreated";
            EventData = JsonSerializer.Serialize(sale);
        }
    }

}