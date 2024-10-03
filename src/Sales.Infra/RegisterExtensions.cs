using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Sales.Domain.Aggregates.SaleAggregate;
using Sales.Domain.Entities;
using Sales.Domain.Entities.Products;
using Sales.Domain.Repositories;
using Sales.Infra.EventSourcing.Repositories;
using Sales.Infra.Persistence.Repositories;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RegisterExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            string mongoConnectionString = configuration["MongoSettings:ConnectionString"];
            string mongoDatabaseName = configuration["MongoSettings:DatabaseName"];

            // MongoDB setup
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client = new MongoClient(mongoConnectionString);
                return client.GetDatabase(mongoDatabaseName);
            });

            // MongoDB repositories
            services.AddScoped<IMongoRepository, MongoRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();

            // SQL Server setup
            services.AddScoped<IDbConnection>(sp =>
            {
                var sqlConnectionString = configuration.GetConnectionString("DefaultConnection");
                return new SqlConnection(sqlConnectionString);
            });

            return services;
        }
    }
}
