using Dapper;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Enums;
using Sales.Domain.Repositories;
using System.Text;

namespace Sales.Infra.Persistence.Database.Specifications
{
    public sealed class GetSalesSpecification : ISpecification<Sale>
    {
        private readonly List<string> _conditions = [];
        private readonly DynamicParameters _parameters = new DynamicParameters();
        private readonly List<string> _orderByFields = [];
        private int? _skip;
        private int? _take;

        public GetSalesSpecification(GetSalesSpecificationContract contract)
        {
             WithId(contract.Id)
            .WithCustomerId(contract.CustomerId)
            .WithBranchId(contract.BranchId)
            .WithSaleDateFrom(contract.SaleDateFrom)
            .WithSaleDateTo(contract.SaleDateTo)
            .WithSaleStatus(contract.SaleStatus)
            .WithMinTotalSaleValue(contract.MinTotalSaleValue)
            .WithMaxTotalSaleValue(contract.MaxTotalSaleValue)
            .WithOrdering(contract.OrderingFields)
            .WithPagination(contract.Skip, contract.Take);
        }

        private GetSalesSpecification WithId(long? id)
        {
            if (id.HasValue)
            {
                _conditions.Add("s.Id = @Id");
                _parameters.Add("Id", id.Value);
            }
            return this;
        }

        private GetSalesSpecification WithCustomerId(long? customerId)
        {
            if (customerId.HasValue)
            {
                _conditions.Add("s.CustomerId = @CustomerId");
                _parameters.Add("CustomerId", customerId.Value);
            }
            return this;
        }

        private GetSalesSpecification WithBranchId(long? branchId)
        {
            if (branchId.HasValue)
            {
                _conditions.Add("s.BranchId = @BranchId");
                _parameters.Add("BranchId", branchId.Value);
            }
            return this;
        }

        private GetSalesSpecification WithSaleDateFrom(DateTime? saleDateFrom)
        {
            if (saleDateFrom.HasValue)
            {
                _conditions.Add("s.SaleDate >= @SaleDateFrom");
                _parameters.Add("SaleDateFrom", saleDateFrom.Value);
            }
            return this;
        }

        private GetSalesSpecification WithSaleDateTo(DateTime? saleDateTo)
        {
            if (saleDateTo.HasValue)
            {
                _conditions.Add("s.SaleDate <= @SaleDateTo");
                _parameters.Add("SaleDateTo", saleDateTo.Value);
            }
            return this;
        }

        private GetSalesSpecification WithSaleStatus(ESaleStatus? saleStatus)
        {
            if (saleStatus.HasValue)
            {
                _conditions.Add("s.SaleStatus = @SaleStatus");
                _parameters.Add("SaleStatus", saleStatus.Value);
            }
            return this;
        }

        private GetSalesSpecification WithMinTotalSaleValue(decimal? minTotalSaleValue)
        {
            if (minTotalSaleValue.HasValue)
            {
                _conditions.Add("s.TotalSaleValue >= @MinTotalSaleValue");
                _parameters.Add("MinTotalSaleValue", minTotalSaleValue.Value);
            }
            return this;
        }

        private GetSalesSpecification WithMaxTotalSaleValue(decimal? maxTotalSaleValue)
        {
            if (maxTotalSaleValue.HasValue)
            {
                _conditions.Add("s.TotalSaleValue <= @MaxTotalSaleValue");
                _parameters.Add("MaxTotalSaleValue", maxTotalSaleValue.Value);
            }
            return this;
        }

        private GetSalesSpecification WithOrdering(ICollection<OrderingField>? orderingFields)
        {
            if (orderingFields != null && orderingFields.Any())
            {
                foreach (var field in orderingFields)
                {
                    if (IsValidOrderingField(field.FieldName))
                        _orderByFields.Add($"{field.FieldName} {(field.Ascending ? "ASC" : "DESC")}");
                }
            }
            return this;
        }

        private bool IsValidOrderingField(string fieldName)
        {
            var allowedFields = new List<string>
            {
                "SaleDate",
                "TotalSaleValue",
                "SaleNumber",
                "Id"
            };

            return allowedFields.Any(x => x.Equals(fieldName));
        }

        private GetSalesSpecification WithPagination(int? skip, int? take)
        {
            _skip = skip;
            _take = take;
            return this;
        }

        public string ToSqlQuery()
        {
            var baseQuery = new StringBuilder();
            baseQuery.AppendLine("SELECT s.*, c.*, b.*, si.*");
            baseQuery.AppendLine("FROM Sale s");
            baseQuery.AppendLine("INNER JOIN Customer c ON s.CustomerId = c.Id");
            baseQuery.AppendLine("INNER JOIN Branch b ON s.BranchId = b.Id");
            baseQuery.AppendLine("INNER JOIN SaleItem si ON s.Id = si.SaleId");

            if (_conditions.Any())
            {
                baseQuery.AppendLine(" WHERE " + string.Join(" AND ", _conditions));
            }

            if (_orderByFields.Any())
            {
                baseQuery.AppendLine(" ORDER BY " + string.Join(", ", _orderByFields));
            }

            if (_skip.HasValue || _take.HasValue)
            {
                if (!_orderByFields.Any())
                {
                    baseQuery.AppendLine("ORDER BY s.Id");
                }

                baseQuery.AppendLine($"OFFSET {_skip.GetValueOrDefault(0)} ROWS");

                if (_take.HasValue)
                {
                    baseQuery.AppendLine($"FETCH NEXT {_take.Value} ROWS ONLY");
                }
            }

            return baseQuery.ToString();
        }

        public object Parameters => _parameters;
    }
}
