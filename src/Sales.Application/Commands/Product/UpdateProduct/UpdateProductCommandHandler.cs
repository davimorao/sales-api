using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;

namespace Sales.Application.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, BaseResponse<Product>>
    {
        private readonly IValidator<UpdateProductCommand> _validator;
        private readonly ILogger<UpdateProductCommand> _logger;
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IValidator<UpdateProductCommand> validator,
                                           ILogger<UpdateProductCommand> logger,
                                           IProductRepository productRepository)
        {
            _validator = validator;
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<BaseResponse<Product>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    return new BaseResponse<Product>(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

                var updatedProduct = new Product
                {
                    Id = request.Id,
                    ProductName = request.ProductName,
                    UnitPrice = request.UnitPrice
                };
                var result = await _productRepository.UpdateAsync(updatedProduct);
                if (!result)
                    return new BaseResponse<Product>("Error while Update Product.");

                return new BaseResponse<Product>(updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while Update Product: {ex.Message}.");
                return new BaseResponse<Product>(ex.Message);
            }
        }
    }
}
