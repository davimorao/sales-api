using MediatR;
using Sales.Domain.Entities;

namespace Sales.Application.Queries.GetProductsByFilters;

public sealed class GetProductsQuery : IRequest<GetProductsResult>
{
    public GetProductsSpecificationContract Contract { get; set; }

    public GetProductsQuery(GetProductsSpecificationContract contract)
    {
        Contract = contract;
    }
}
