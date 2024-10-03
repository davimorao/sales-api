using Sales.Domain.Entities;
using System.Text.Json;

namespace Sales.Domain.Aggregates.SaleAggregate.Events
{
    public sealed class SaleUpdatedEvent : DomainEvent
    {
        public SaleUpdatedEvent(Sale sale)
        {
            AggregateType = "Sale";
            EventType = "SaleUpdated";
            EventData = JsonSerializer.Serialize(sale);
        }
    }
}
