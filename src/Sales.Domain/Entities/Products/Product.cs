using Dapper.Contrib.Extensions;

namespace Sales.Domain.Entities
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public long Id { get; set; }
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
