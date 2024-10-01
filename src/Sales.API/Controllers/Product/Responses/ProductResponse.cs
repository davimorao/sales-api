using Sales.Application;
using Sales.Application.Queries.GetProductsByFilters;
using Sales.Domain.Entities;

namespace Sales.API.Controllers.Requests
{
    public class ProductResponse
    {
        public long Id { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }

        public static ProductResponse FromResult(BaseResponse<Product> product)
        {
            return new ProductResponse
            {
                Id = product.Data.Id,
                ProductName = product.Data.ProductName,
                UnitPrice = product.Data.UnitPrice
            };
        }

        public static IEnumerable<ProductResponse> FromResult(GetProductsResult result)
        {
            return result.Products.Select(product => new ProductResponse
            {
                Id = product.Id,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice
            });
        }
    }
}
