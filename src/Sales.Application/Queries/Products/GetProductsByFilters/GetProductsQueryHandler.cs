using MediatR;
using Sales.Domain.Repositories;
using Sales.Infra.Persistence.Database.Specifications;

namespace Sales.Application.Queries.GetProductsByFilters;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, GetProductsResult>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<GetProductsResult> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetProductsSpecification(request.Contract);
        var sales = await _productRepository.GetBySpecificationAsync(specification);

        return new GetProductsResult
        {
            Products = sales
        };
    }
}
