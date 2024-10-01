using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Application.Messaging;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Aggregates.SaleAggregate.Events;
using Sales.Domain.Enums;
using Sales.Domain.Repositories;

namespace Sales.Application.Commands
{
    public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, BaseResponse<Sale>>
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
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    return new BaseResponse<Sale>(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

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

                _ = await _saleRepository.InsertAsync(sale);

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
