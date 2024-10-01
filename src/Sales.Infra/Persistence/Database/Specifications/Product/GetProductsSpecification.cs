using Dapper;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;
using System.Text;

namespace Sales.Infra.Persistence.Database.Specifications
{
    public sealed class GetProductsSpecification : ISpecification<Product>
    {
        private readonly List<string> _conditions = new List<string>();
        private readonly DynamicParameters _parameters = new DynamicParameters();
        private readonly List<string> _orderByFields = new List<string>();
        private int? _skip;
        private int? _take;

        public GetProductsSpecification(GetProductsSpecificationContract contract)
        {
            WithId(contract.Id)
            .WithProductName(contract.ProductName)
            .WithUnitPrice(contract.UnitPrice)
            .WithOrdering(contract.OrderingFields)
            .WithPagination(contract.Skip, contract.Take);
        }

        private GetProductsSpecification WithId(long? id)
        {
            if (id.HasValue)
            {
                _conditions.Add("p.Id = @Id");
                _parameters.Add("Id", id);
            }
            return this;
        }

        private GetProductsSpecification WithProductName(string productName)
        {
            if (!string.IsNullOrEmpty(productName))
            {
                _conditions.Add("p.ProductName LIKE @ProductName");
                _parameters.Add("ProductName", $"%{productName}%");
            }
            return this;
        }

        private GetProductsSpecification WithUnitPrice(decimal unitPrice)
        {
            if (unitPrice > 0)
            {
                _conditions.Add("p.UnitPrice = @UnitPrice");
                _parameters.Add("UnitPrice", unitPrice);
            }
            return this;
        }

        private GetProductsSpecification WithOrdering(List<OrderingField>? orderingFields)
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
                "ProductName",
                "UnitPrice",
                "Id"
            };

            return allowedFields.Any(x => x.Equals(fieldName));
        }

        private GetProductsSpecification WithPagination(int? skip, int? take)
        {
            _skip = skip;
            _take = take;
            return this;
        }

        public string ToSqlQuery()
        {
            var baseQuery = new StringBuilder();
            baseQuery.AppendLine("SELECT p.*");
            baseQuery.AppendLine("FROM Product p");

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
                    baseQuery.AppendLine("ORDER BY p.Id");
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
