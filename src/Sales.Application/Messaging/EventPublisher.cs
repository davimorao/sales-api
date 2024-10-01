using MassTransit;
using Microsoft.Extensions.Logging;

namespace Sales.Application.Messaging
{
    public sealed class EventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(IPublishEndpoint publishEndpoint, ILogger<EventPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            if (@event is null)
            {
                _logger.LogWarning("Event is null.");
                return;
            }

            await _publishEndpoint.Publish(@event);
            _logger.LogInformation($"Event of type {typeof(T).Name} published successfully.");
        }
    }
}
