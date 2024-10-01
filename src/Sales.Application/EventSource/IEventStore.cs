namespace Sales.Application.EventSource
{
    public interface IEventStore
    {
        Task SaveEventAsync<T>(T @event) where T : class;
        Task<IEnumerable<T>> GetEventsAsync<T>() where T : class;
    }
}
