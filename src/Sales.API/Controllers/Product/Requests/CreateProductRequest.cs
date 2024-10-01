using Sales.Application.Commands;

namespace Sales.API.Controllers.Requests
{
    public class CreateProductRequest
    {
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }

        public CreateProductCommand ToCommand()
        {
            return new CreateProductCommand
            {
                ProductName = ProductName,
                UnitPrice = UnitPrice
            };
        }
    }
}
