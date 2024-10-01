using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Application.Messaging;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Aggregates.SaleAggregate.Events;
using Sales.Domain.Enums;

namespace Sales.Application.Commands
{
    public sealed class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, BaseResponse<Sale>>
    {
        private readonly IValidator<CreateSaleCommand> _validator;
        private readonly ILogger<CreateSaleCommand> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IEventPublisher _eventPublisher;

        public CreateSaleCommandHandler(IValidator<CreateSaleCommand> validator,
                                        ILogger<CreateSaleCommand> logger,
                                        ISaleRepository saleRepository,
                                        IEventPublisher eventPublisher)
        {
            _validator = validator;
            _logger = logger;
            _saleRepository = saleRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<BaseResponse<Sale>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting validation for CreateSaleCommand.");
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogError("Validation failed for CreateSaleCommand: {ValidationErrors}",
                                       validationResult.Errors.Select(e => e.ErrorMessage));
                    return new BaseResponse<Sale>(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }

                _logger.LogInformation("Building Sale entity for the CreateSaleCommand.");
                var sale = new Sale
                {
                    SaleNumber = Guid.NewGuid().ToString()[..20],
                    SaleStatus = ESaleStatus.Active,
                    SaleDate = request.SaleDate,
                    CustomerId = request.CustomerId,
                    BranchId = request.BranchId,
                    Items = request.Items.Select(item => new SaleItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        TotalItemValue = (item.UnitPrice * item.Quantity) - item.Discount
                    }).ToList()
                };
                sale.TotalSaleValue = sale.Items.Sum(item => item.TotalItemValue);

                _logger.LogInformation("Inserting sale into the repository.");
                _ = await _saleRepository.InsertAsync(sale);
                _logger.LogInformation("Sale inserted successfully with Id: {SaleId}", sale.Id);

                var saleCreatedEvent = new SaleCreatedEvent
                {
                    Id = sale.Id,
                    CustomerId = sale.CustomerId,
                    BranchId = sale.BranchId,
                    SaleStatus = sale.SaleStatus
                };
                await _eventPublisher.PublishAsync(saleCreatedEvent);

                return new BaseResponse<Sale>(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while creating Sale: {ex.Message}.");
                return new BaseResponse<Sale>(ex.Message);
            }
        }
    }
}
