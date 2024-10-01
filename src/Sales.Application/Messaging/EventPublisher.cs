using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Sales.Application.Messaging
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(ILogger<EventPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync<T>(T @event)
        {
            if (@event != null)
            {
                var eventData = JsonSerializer.Serialize(@event);
                _logger.LogInformation($"Event of type {typeof(T).Name} with content {eventData} published successfully.");
            }

            return Task.CompletedTask;
        }
    }
}
