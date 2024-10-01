using Sales.Application.Commands;

namespace Sales.API.Controllers.Requests
{
    public class UpdateProductRequest
    {
        public long Id { get; set; } // Set from the route
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }

        public UpdateProductCommand ToCommand()
        {
            return new UpdateProductCommand
            {
                Id = Id,
                ProductName = ProductName,
                UnitPrice = UnitPrice
            };
        }
    }
}
