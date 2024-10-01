using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Sales.Application.Messaging
{
    public class CommandPublisher : ICommandPublisher
    {
        private readonly ILogger<CommandPublisher> _logger;

        public CommandPublisher(ILogger<CommandPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync<T>(T command)
        {
            if (command != null)
            {
                var commandData = JsonSerializer.Serialize(command);
                _logger.LogInformation($"Command of type {typeof(T).Name} with content {commandData} published successfully.");
            }

            return Task.CompletedTask;
        }
    }
}
