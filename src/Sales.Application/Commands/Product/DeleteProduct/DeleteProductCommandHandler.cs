using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Domain.Entities.Products;

namespace Sales.Application.Commands
{
    public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, BaseResponse<bool>>
    {
        private readonly IValidator<DeleteProductCommand> _validator;
        private readonly ILogger<DeleteProductCommand> _logger;
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IValidator<DeleteProductCommand> validator,
                                           ILogger<DeleteProductCommand> logger,
                                           IProductRepository productRepository)
        {
            _validator = validator;
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<BaseResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting validation for DeleteProductCommand with Id: {ProductId}", request.Id);
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogError("Validation failed for DeleteProductCommand: {ValidationErrors}",
                                       validationResult.Errors.Select(e => e.ErrorMessage));
                    return new BaseResponse<bool>(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }

                _logger.LogInformation("Deleting product from repository with Id: {ProductId}", request.Id);
                var result = await _productRepository.DeleteByIdAsync(request.Id);
                if (!result)
                {
                    _logger.LogError("Failed to delete product with Id: {ProductId}", request.Id);
                    return new BaseResponse<bool>("Error while Delete Product.");
                }

                _logger.LogInformation("Product with Id: {ProductId} deleted successfully", request.Id);
                return new BaseResponse<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while Delete Product: {ex.Message}.");
                return new BaseResponse<bool>(ex.Message);
            }
        }
    }
}
