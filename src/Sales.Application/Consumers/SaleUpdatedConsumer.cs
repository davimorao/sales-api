using MassTransit;
using Microsoft.Extensions.Logging;
using Sales.Application.EventSource;
using Sales.Domain.Aggregates.SaleAggregate.Events;

namespace Sales.Application.Consumers
{
    public sealed class SaleUpdatedConsumer : IConsumer<SaleUpdatedEvent>
    {
        private readonly ILogger<SaleUpdatedConsumer> _logger;
        private readonly IEventStore _eventStore;

        public SaleUpdatedConsumer(ILogger<SaleUpdatedConsumer> logger, IEventStore eventStore)
        {
            _logger = logger;
            _eventStore = eventStore;
        }

        public async Task Consume(ConsumeContext<SaleUpdatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Event consumed with Id: {message.Id} and Name: {message.SaleStatus}");

            // Adicione aqui a lógica de processamento do evento

            // Persistir na event store
            await _eventStore.SaveEventAsync(message);

            var events = await _eventStore.GetEventsAsync<SaleUpdatedEvent>();
            _logger.LogInformation($"event store: {events}");

        }
    }
}
