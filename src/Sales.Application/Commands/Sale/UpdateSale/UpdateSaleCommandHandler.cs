using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Application.Messaging;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Aggregates.SaleAggregate.Events;
using Sales.Domain.Repositories;

namespace Sales.Application.Commands
{
    public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, BaseResponse<Sale>>
    {
        private readonly IValidator<UpdateSaleCommand> _validator;
        private readonly ILogger<UpdateSaleCommand> _logger;
        private readonly ISaleRepository _saleRepository;
        private readonly IEventPublisher _eventPublisher;

        public UpdateSaleCommandHandler(IValidator<UpdateSaleCommand> validator,
                                        ILogger<UpdateSaleCommand> logger,
                                        ISaleRepository saleRepository,
                                        IEventPublisher eventPublisher)
        {
            _validator = validator;
            _logger = logger;
            _saleRepository = saleRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<BaseResponse<Sale>> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    return new BaseResponse<Sale>(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

                var sale = await _saleRepository.GetByIdAsync(request.Id);
                if (sale == null)
                    return new BaseResponse<Sale>("Sale not found.");

                if(request.SaleDate.HasValue)
                    sale.SaleDate = request.SaleDate.Value;

                if (request.CustomerId.HasValue)
                    sale.CustomerId = request.CustomerId.Value;

                if (request.BranchId.HasValue)
                    sale.BranchId = request.BranchId.Value;

                if (request.SaleStatus.HasValue)
                    sale.SaleStatus = request.SaleStatus.Value;

                if (request.Items.Any())
                {
                    sale.Items = request.Items.Select(item => new SaleItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        TotalItemValue = (item.UnitPrice * item.Quantity) - item.Discount
                    }).ToList();
                    sale.TotalSaleValue = sale.Items.Sum(item => item.TotalItemValue);
                }

                var result = await _saleRepository.UpdateAsync(sale);
                if (!result)
                    return new BaseResponse<Sale>("Error while updating Sale.");

                if(sale.SaleStatus == Domain.Enums.ESaleStatus.Cancelled)
                {
                    await _eventPublisher.PublishAsync(new SaleCancelledEvent
                    {
                        Id = sale.Id,
                        CustomerId = sale.CustomerId,
                        BranchId = sale.BranchId,
                        SaleStatus = sale.SaleStatus
                    });
                }
                else
                {
                    await _eventPublisher.PublishAsync(new SaleUpdatedEvent
                    {
                        Id = sale.Id,
                        CustomerId = sale.CustomerId,
                        BranchId = sale.BranchId,
                        SaleStatus = sale.SaleStatus
                    });
                }

                return new BaseResponse<Sale>(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while updating Sale: {ex.Message}.");
                return new BaseResponse<Sale>(ex.Message);
            }
        }
    }
}
