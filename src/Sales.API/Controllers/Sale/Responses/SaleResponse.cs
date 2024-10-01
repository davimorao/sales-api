using Sales.Application;
using Sales.Application.Queries.GetSalesByFilters;
using Sales.Domain.Enums;

namespace Sales.API.Controllers.Responses
{
    public class SaleResponse
    {
        public long Id { get; set; }
        public required string SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalSaleValue { get; set; }
        public ESaleStatus SaleStatusCode { get; set; }
        public string SaleStatusDescription => SaleStatusCode.ToString();
        public long CustomerId { get; set; }
        public required string CustomerName { get; set; }
        public long BranchId { get; set; }
        public required string BranchName { get; set; }
        public IEnumerable<SaleItemResponse> Items { get; set; } = [];

        public static SaleResponse FromResult(BaseResponse<Sales.Domain.Aggregates.SaleAggregate.Sale> sale)
        {
            return new SaleResponse
            {
                Id = sale.Data.Id,
                SaleNumber = sale.Data.SaleNumber,
                SaleDate = sale.Data.SaleDate,
                TotalSaleValue = sale.Data.TotalSaleValue,
                SaleStatusCode = sale.Data.SaleStatus,
                CustomerId = sale.Data.CustomerId,
                CustomerName = sale.Data.Customer?.CustomerName,
                BranchId = sale.Data.BranchId,
                BranchName = sale.Data.Branch?.BranchName,
                Items = sale.Data.Items.Select(item => new SaleItemResponse
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TotalItemValue = item.TotalItemValue
                })
            };
        }


        public static IEnumerable<SaleResponse> FromResult(GetSalesResult result)
        {
            return result.Sales.Select(sale => new SaleResponse
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                TotalSaleValue = sale.TotalSaleValue,
                SaleStatusCode = sale.SaleStatus,
                CustomerId = sale.CustomerId,
                CustomerName = sale.Customer?.CustomerName,
                BranchId = sale.BranchId,
                BranchName = sale.Branch?.BranchName,
                Items = sale.Items.Select(x => new SaleItemResponse
                {
                    Id = x.Id,
                    SaleId = x.SaleId,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Discount = x.Discount,
                    TotalItemValue = x.TotalItemValue
                })
            });
        }
    }
}
