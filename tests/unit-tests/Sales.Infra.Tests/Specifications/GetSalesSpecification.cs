using Bogus;
using Dapper;
using FluentAssertions;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Enums;
using Sales.Domain.Repositories;
using Sales.Infra.Persistence.Specifications;

namespace Sales.Infra.Tests.Specifications
{
    public class GetSalesSpecificationTests
    {
        private readonly Faker<GetSalesSpecificationContract> _contractFaker;

        public GetSalesSpecificationTests()
        {
            _contractFaker = new Faker<GetSalesSpecificationContract>()
                .RuleFor(c => c.CustomerId, f => f.Random.Long(1, 1000))
                .RuleFor(c => c.BranchId, f => f.Random.Long(1, 1000))
                .RuleFor(c => c.SaleDateFrom, f => f.Date.Past())
                .RuleFor(c => c.SaleDateTo, f => f.Date.Recent())
                .RuleFor(c => c.SaleStatus, f => f.PickRandom<ESaleStatus>())
                .RuleFor(c => c.MinTotalSaleValue, f => f.Random.Decimal(100, 1000))
                .RuleFor(c => c.MaxTotalSaleValue, f => f.Random.Decimal(1000, 5000))
                .RuleFor(c => c.Skip, f => f.Random.Int(0, 100))
                .RuleFor(c => c.Take, f => f.Random.Int(1, 100))
                .RuleFor(c => c.OrderingFields, f => new List<OrderingField>
                {
                    new OrderingField { FieldName = "SaleDate", Ascending = true }
                });
        }

        [Fact]
        public void ToSqlQuery_WhenFilteringByCustomerId_ShouldIncludeCustomerIdCondition()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            contract.BranchId = null;
            var specification = new GetSalesSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("s.CustomerId = @CustomerId");
            ((DynamicParameters)specification.Parameters).Get<long>("CustomerId").Should().Be(contract.CustomerId.Value);
        }

        [Fact]
        public void ToSqlQuery_WhenFilteringByBranchId_ShouldIncludeBranchIdCondition()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            contract.CustomerId = null;
            var specification = new GetSalesSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("s.BranchId = @BranchId");
            ((DynamicParameters)specification.Parameters).Get<long>("BranchId").Should().Be(contract.BranchId.Value);
        }

        [Fact]
        public void ToSqlQuery_WhenFilteringBySaleDateRange_ShouldIncludeSaleDateConditions()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            var specification = new GetSalesSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("s.SaleDate >= @SaleDateFrom");
            query.Should().Contain("s.SaleDate <= @SaleDateTo");
            ((DynamicParameters)specification.Parameters).Get<DateTime>("SaleDateFrom").Should().Be(contract.SaleDateFrom.Value);
            ((DynamicParameters)specification.Parameters).Get<DateTime>("SaleDateTo").Should().Be(contract.SaleDateTo.Value);
        }

        [Fact]
        public void ToSqlQuery_WhenFilteringBySaleStatus_ShouldIncludeSaleStatusCondition()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            var specification = new GetSalesSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("s.SaleStatus = @SaleStatus");
            ((DynamicParameters)specification.Parameters).Get<ESaleStatus>("SaleStatus").Should().Be(contract.SaleStatus.Value);
        }

        [Fact]
        public void ToSqlQuery_WhenFilteringByTotalSaleValueRange_ShouldIncludeTotalSaleValueConditions()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            var specification = new GetSalesSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("s.TotalSaleValue >= @MinTotalSaleValue");
            query.Should().Contain("s.TotalSaleValue <= @MaxTotalSaleValue");
            ((DynamicParameters)specification.Parameters).Get<decimal>("MinTotalSaleValue").Should().Be(contract.MinTotalSaleValue!.Value);
            ((DynamicParameters)specification.Parameters).Get<decimal>("MaxTotalSaleValue").Should().Be(contract.MaxTotalSaleValue!.Value);
        }

        [Fact]
        public void ToSqlQuery_WhenOrderingBySaleDate_ShouldIncludeOrderByClause()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            var specification = new GetSalesSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("ORDER BY SaleDate ASC");
        }

        [Fact]
        public void ToSqlQuery_WhenPaging_ShouldIncludeOffsetAndFetchNextClause()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            var specification = new GetSalesSpecification(contract);

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
            var specification = new GetSalesSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().NotContain("ORDER BY InvalidField");
        }

        [Fact]
        public void ToSqlQuery_WhenNoConditionsProvided_ShouldReturnBaseQuery()
        {
            // Arrange
            var contract = new GetSalesSpecificationContract();
            var specification = new GetSalesSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().StartWith("SELECT s.*, c.*, b.*, si.*");
            query.Should().Contain("FROM Sale s");
        }

        [Fact]
        public void ToSqlQuery_WhenMultipleConditionsProvided_ShouldIncludeAllConditions()
        {
            // Arrange
            var contract = _contractFaker.Generate();
            var specification = new GetSalesSpecification(contract);

            // Act
            var query = specification.ToSqlQuery();

            // Assert
            query.Should().Contain("s.CustomerId = @CustomerId");
            query.Should().Contain("s.BranchId = @BranchId");
            query.Should().Contain("s.SaleDate >= @SaleDateFrom");
            query.Should().Contain("s.SaleDate <= @SaleDateTo");
        }
    }
}
