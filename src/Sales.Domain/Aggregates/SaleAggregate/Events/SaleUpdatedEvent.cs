using Sales.Domain.Enums;

namespace Sales.Domain.Aggregates.SaleAggregate.Events
{
    public class SaleUpdatedEvent
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long BranchId { get; set; }
        public ESaleStatus SaleStatus { get; set; }
    }
}
