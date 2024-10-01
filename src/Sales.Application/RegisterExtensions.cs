using FluentValidation;
using MassTransit;
using Sales.Application.Commands;
using Sales.Application.Consumers;
using Sales.Application.EventSource;
using Sales.Application.Messaging;
using Sales.Application.Queries.GetSalesByFilters;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RegisterExtensions
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetSalesQueryHandler).Assembly);
            });
            services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();

            return services;
        }

        public static IServiceCollection AddMessageQueue(this IServiceCollection services)
        {
            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddScoped<ICommandPublisher, CommandPublisher>();
            services.AddSingleton<IEventStore, InMemoryEventStore>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<SaleUpdatedConsumer>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}
