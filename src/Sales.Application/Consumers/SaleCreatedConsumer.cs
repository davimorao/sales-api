using MassTransit;
using Microsoft.Extensions.Logging;
using Sales.Domain.Aggregates.SaleAggregate.Events;
using Sales.Domain.Repositories;

namespace Sales.Application.Consumers
{
    public class SaleCreatedConsumer : IConsumer<SaleCreatedEvent>
    {
        private readonly ILogger<SaleCreatedConsumer> _logger;
        private readonly IMongoRepository _mongoRepository;

        public SaleCreatedConsumer(ILogger<SaleCreatedConsumer> logger, IMongoRepository mongoRepository)
        {
            _logger = logger;
            _mongoRepository = mongoRepository;
        }

        public async Task Consume(ConsumeContext<SaleCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Event consumed with Id: {message.Id}");

            await _mongoRepository.InsertAsync(message);

            _logger.LogInformation("Event persisted in MongoDB");

            var events = await _mongoRepository.GetEventsByAggregateTypeAsync("Sale");
            _logger.LogInformation($"Event store count: {events?.Count()}");
        }
    }
}
