using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Infra.Persistence.Specifications;

namespace Sales.Application.Queries.GetSalesByFilters;

public sealed class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, GetSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<GetSalesQueryHandler> _logger;


    public GetSalesQueryHandler(ISaleRepository saleRepository, ILogger<GetSalesQueryHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }

    public async Task<GetSalesResult> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Building specification for sales query.");
        var specification = new GetSalesSpecification(request.Contract);

        _logger.LogInformation("Retrieving sales from repository based on specification.");
        var sales = await _saleRepository.GetBySpecificationWithRelationShipAsync(specification);
        _logger.LogInformation("Sales retrieved successfully from repository. Total sales found: {SalesCount}", sales.Count());

        return new GetSalesResult
        {
            Sales = sales
        };
    }
}
