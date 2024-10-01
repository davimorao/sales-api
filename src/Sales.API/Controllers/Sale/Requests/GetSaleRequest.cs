using Sales.Application.Queries.GetSalesByFilters;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Enums;

namespace Sales.API.Controllers.Requests
{
    public class GetSaleRequest
    {
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        public long? BranchId { get; set; }
        public DateTime? SaleDateFrom { get; set; }
        public DateTime? SaleDateTo { get; set; }
        public ESaleStatus? SaleStatus { get; set; }

        public int? Skip { get; set; }
        public int? Take { get; set; }

        public GetSalesQuery ToCommand() => new(new GetSalesSpecificationContract
        {
            Id = Id,
            CustomerId = CustomerId,
            BranchId = BranchId,
            SaleDateFrom = SaleDateFrom,
            SaleDateTo = SaleDateTo,
            SaleStatus = SaleStatus,
            Skip = Skip,
            Take = Take
        });
    }
}
