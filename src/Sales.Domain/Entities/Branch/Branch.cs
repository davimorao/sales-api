using Dapper.Contrib.Extensions;
using Sales.Domain.Aggregates.SaleAggregate;

namespace Sales.Domain.Entities
{
    public sealed class Branch
    {
        public long Id { get; set; }
        public string? BranchName { get; set; }

        // Navigation properties
        [Write(false)]
        public List<Sale>? Sales { get; set; }
    }
}