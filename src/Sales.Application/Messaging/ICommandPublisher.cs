
namespace Sales.Application.Messaging
{
    public interface ICommandPublisher
    {
        Task PublishAsync<T>(T command);
    }
}
