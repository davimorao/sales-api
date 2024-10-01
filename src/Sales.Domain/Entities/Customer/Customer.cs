using Dapper.Contrib.Extensions;
using Sales.Domain.Aggregates.SaleAggregate;

namespace Sales.Domain.Entities
{
    [Table("Customer")]
    public class Customer
    {
        public long Id { get; set; }
        public string? CustomerName { get; set; }

        // Navigation property
        [Write(false)]
        public List<Sale>? Sales { get; set; }
    }
}
