using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;

namespace Sales.Application.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, BaseResponse<Product>>
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
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    return new BaseResponse<Product>(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

                var createdProduct = new Product
                {
                    ProductName = request.ProductName,
                    UnitPrice = request.UnitPrice
                };
                _ = await _productRepository.InsertAsync(createdProduct);

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
