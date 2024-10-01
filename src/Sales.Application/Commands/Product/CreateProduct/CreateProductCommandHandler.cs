using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Domain.Entities;
using Sales.Domain.Entities.Products;

namespace Sales.Application.Commands
{
    public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, BaseResponse<Product>>
    {
        private readonly IValidator<CreateProductCommand> _validator;
        private readonly ILogger<CreateProductCommand> _logger;
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IValidator<CreateProductCommand> validator,
                                           ILogger<CreateProductCommand> logger,
                                           IProductRepository productRepository)
        {
            _validator = validator;
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<BaseResponse<Product>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting validation for CreateProductCommand");
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogError("Validation failed for CreateProductCommand: {ValidationErrors}",
                                       validationResult.Errors.Select(e => e.ErrorMessage));
                    return new BaseResponse<Product>(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }

                var createdProduct = new Product
                {
                    ProductName = request.ProductName,
                    UnitPrice = request.UnitPrice
                };

                _logger.LogInformation("Inserting product into the repository");
                _ = await _productRepository.InsertAsync(createdProduct);
                _logger.LogInformation("Product inserted successfully with Id: {ProductId}", createdProduct.Id);

                return new BaseResponse<Product>(createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while Create Product: {ex.Message}.");
                return new BaseResponse<Product>(ex.Message);
            }
        }
    }
}
