
namespace OrderService.Api.Infrastructure.Messaging
{
    public interface IOrderCreatedEventSenderService
    {
        Task CloseQueueAsync();
        Task PublishOrderCreatedEvent(Order order);
    }
}