﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Domain.Repositories;

namespace Sales.Application.Commands
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, BaseResponse<bool>>
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
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    return new BaseResponse<bool>(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

                var result = await _productRepository.DeleteByIdAsync(request.Id);
                if (!result)
                    return new BaseResponse<bool>("Error while Delete Product.");

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
