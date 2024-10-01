using MediatR;
using Microsoft.Extensions.Logging;
using Sales.Domain.Entities.Products;
using Sales.Infra.Persistence.Database.Specifications;

namespace Sales.Application.Queries.GetProductsByFilters;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, GetProductsResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(IProductRepository productRepository, ILogger<GetProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<GetProductsResult> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Building specification for sales query.");
        var specification = new GetProductsSpecification(request.Contract);

        _logger.LogInformation("Retrieving sales from repository based on specification.");
        var products = await _productRepository.GetBySpecificationAsync(specification);
        _logger.LogInformation("Products retrieved successfully from repository. Total products found: {ProductsCount}", products.Count());

        return new GetProductsResult
        {
            Products = products
        };
    }
}
