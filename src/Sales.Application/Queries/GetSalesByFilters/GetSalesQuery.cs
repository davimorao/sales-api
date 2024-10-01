using MediatR;
using Sales.Domain.Aggregates.SaleAggregate;

namespace Sales.Application.Queries.GetSalesByFilters;

public sealed class GetSalesQuery : IRequest<GetSalesResult>
{
    public GetSalesSpecificationContract Contract { get; set; }

    public GetSalesQuery(GetSalesSpecificationContract contract)
    {
        Contract = contract;
    }
}
