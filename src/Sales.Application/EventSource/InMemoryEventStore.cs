namespace Sales.Application.EventSource
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<object> _events = new();

        public Task SaveEventAsync<T>(T @event) where T : class
        {
            _events.Add(@event);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> GetEventsAsync<T>() where T : class
        {
            var filteredEvents = _events.OfType<T>();
            return Task.FromResult(filteredEvents);
        }
    }
}
