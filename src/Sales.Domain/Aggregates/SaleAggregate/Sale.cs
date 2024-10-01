using Dapper.Contrib.Extensions;
using Sales.Domain.Entities;
using Sales.Domain.Enums;

namespace Sales.Domain.Aggregates.SaleAggregate
{
    [Table("Sale")]
    public class Sale
    {
        [Key]
        public long Id { get; set; }
        public string? SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public long CustomerId { get; set; }
        public long BranchId { get; set; }
        public decimal TotalSaleValue { get; set; }
        public ESaleStatus SaleStatus { get; set; }

        // Navigation properties
        [Write(false)]
        public Customer? Customer { get; set; }
        [Write(false)]
        public Branch? Branch { get; set; }
        [Write(false)]
        public List<SaleItem> Items { get; set; } = [];
    }
}
