using Dapper.Contrib.Extensions;
using Sales.Domain.Entities;

namespace Sales.Domain.Aggregates.SaleAggregate
{
    [Table("SaleItem")]
    public class SaleItem
    {
        [Key]
        public long Id { get; set; }
        public long SaleId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        [Write(false)]
        public decimal TotalItemValue { get; set; }

        // Navigation properties
        [Write(false)]
        public Sale? Sale { get; set; }
        [Write(false)]
        public Product? Product { get; set; }
    }
}
