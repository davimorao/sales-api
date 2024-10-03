using Dapper;
using Dapper.Contrib.Extensions;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;
using System.Data;

namespace Sales.Infra.Persistence.Repositories
{
    public sealed class SaleRepository(IDbConnection dbConnection) : SqlRepository<Sale>(dbConnection), ISaleRepository
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<IEnumerable<Sale>> GetBySpecificationWithRelationShipAsync(ISqlSpecification<Sale> specification)
        {
            var sql = specification.ToSqlQuery();
            var parameters = specification.Parameters;

            var salesDictionary = new Dictionary<long, Sale>();

            var sales = await _dbConnection.QueryAsync<Sale, Customer, Branch, SaleItem, Sale>(
                sql,
                (sale, customer, branch, salesItem) =>
                {
                    if (!salesDictionary.TryGetValue(sale.Id, out var saleEntry))
                    {
                        sale.Customer = customer;
                        sale.Branch = branch;
                        sale.Items = new List<SaleItem>();
                        salesDictionary[sale.Id] = sale;
                        saleEntry = sale;
                    }

                    if (salesItem != null)
                        saleEntry.Items.Add(salesItem);

                    return saleEntry;
                },
                param: parameters,
                splitOn: "Id,Id,Id"
            );

            return salesDictionary.Values;
        }

        public override async Task<long> InsertAsync(Sale entity)
        {
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                var saleId = await _dbConnection.InsertAsync(entity, transaction);

                var saleItems = entity.Items.Select(item =>
                {
                    item.SaleId = saleId;
                    return item;
                }).ToList();

                await _dbConnection.InsertAsync(saleItems, transaction);

                transaction.Commit();
                return saleId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally { _dbConnection.Close(); }
        }

        public override async Task<bool> UpdateAsync(Sale entity)
        {
            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                await _dbConnection.UpdateAsync(entity, transaction);

                var deleteSaleItemsSql = $"DELETE FROM {nameof(SaleItem)} WHERE SaleId = @SaleId;";
                await _dbConnection.ExecuteAsync(deleteSaleItemsSql, new { SaleId = entity.Id }, transaction);

                foreach (var item in entity.Items)
                {
                    item.SaleId = entity.Id;
                    await _dbConnection.InsertAsync(item, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally { _dbConnection.Close(); }
        }

    }
}
