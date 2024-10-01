using MediatR;
using Sales.Domain.Repositories;
using Sales.Infra.Persistence.Database.Specifications;

namespace Sales.Application.Queries.GetSalesByFilters;

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, GetSalesResult>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalesQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<GetSalesResult> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetSalesSpecification(request.Contract);
        var sales = await _saleRepository.GetBySpecificationWithRelationShipAsync(specification);

        return new GetSalesResult
        {
            Sales = sales
        };
    }
}
