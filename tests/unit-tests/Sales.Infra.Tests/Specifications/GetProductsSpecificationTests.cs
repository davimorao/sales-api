using Bogus;
using Dapper;
using FluentAssertions;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;
using Sales.Infra.Persistence.Specifications;

namespace Sales.Infra.Tests.Specifications
{
    public class GetProductsSpecificationTests
    {
        private readonly Faker<GetProductsSpecificationContract> _contractFaker;

        public GetProductsSpecificationTests()
        {
            _contractFaker = new Faker<GetProductsSpecificationContract>()
                .RuleFor(c => c.Id, f => f.Random.Long(1, 1000))
                .RuleFor(c => c.ProductName, f => f.Commerce.ProductName())
                .RuleFor(c => c.UnitPrice, f => f.Random.Decimal(1, 1000))
                .RuleFor(c => c.Skip, f => f.Random.Int(0, 100))
                .RuleFor(c => c.Take, f => f.Random.Int(1, 100))
                .RuleFor(c => c.OrderingFields, f => new List<OrderingField>
                {
                    new OrderingField { FieldName = "ProductName", Ascending = true }
                });
        }

        [Fact]
        public void ToSqlQuery_WhenFilteringById_ShouldIncludeIdCondition()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            contract.ProductName = null;
            contract.UnitPrice = 0;
            var specification = new GetProductsSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("p.Id = @Id");
            ((DynamicParameters)specification.Parameters).Get<long>("Id").Should().Be(contract.Id.Value);
        }

        [Fact]
        public void ToSqlQuery_WhenFilteringByProductName_ShouldIncludeProductNameCondition()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            contract.Id = null;
            contract.UnitPrice = 0;
            var specification = new GetProductsSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("p.ProductName LIKE @ProductName");
            ((DynamicParameters)specification.Parameters).Get<string>("ProductName").Should().Be($"%{contract.ProductName}%");
        }

        [Fact]
        public void ToSqlQuery_WhenFilteringByUnitPrice_ShouldIncludeUnitPriceCondition()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            contract.Id = null;
            contract.ProductName = null;
            var specification = new GetProductsSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("p.UnitPrice = @UnitPrice");
            ((DynamicParameters)specification.Parameters).Get<decimal>("UnitPrice").Should().Be(contract.UnitPrice);
        }

        [Fact]
        public void ToSqlQuery_WhenOrderingByProductName_ShouldIncludeOrderByClause()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            var specification = new GetProductsSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("ORDER BY ProductName ASC");
        }

        [Fact]
        public void ToSqlQuery_WhenPaging_ShouldIncludeOffsetAndFetchNextClause()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            var specification = new GetProductsSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain($"OFFSET {contract.Skip.GetValueOrDefault(0)} ROWS");
            query.Should().Contain($"FETCH NEXT {contract.Take.GetValueOrDefault(0)} ROWS ONLY");
        }

        [Fact]
        public void ToSqlQuery_WhenOrderingByInvalidField_ShouldNotIncludeInvalidOrderByClause()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            contract.OrderingFields = new List<OrderingField>
            {
                new OrderingField { FieldName = "InvalidField", Ascending = true }
            };
            var specification = new GetProductsSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().NotContain("ORDER BY InvalidField");
        }

        [Fact]
        public void ToSqlQuery_WhenNoConditionsProvided_ShouldReturnBaseQuery()
        {
            // Arrange
            var contract = new GetProductsSpecificationContract();
            var specification = new GetProductsSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().StartWith("SELECT p.*");
            query.Should().Contain("FROM Product p");
        }

        [Fact]
        public void ToSqlQuery_WhenMultipleConditionsProvided_ShouldIncludeAllConditions()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            var specification = new GetProductsSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("p.Id = @Id");
            query.Should().Contain("p.ProductName LIKE @ProductName");
            query.Should().Contain("p.UnitPrice = @UnitPrice");
        }
    }
}