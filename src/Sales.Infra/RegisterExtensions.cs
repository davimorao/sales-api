using Microsoft.Extensions.Configuration;
using Sales.Domain.Repositories;
using Sales.Infra.Persistence.Database.Repositories;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RegisterExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();

            services.AddScoped<IDbConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                return new SqlConnection(connectionString);
            });
            return services;
        }
    }
}
