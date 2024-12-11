
namespace OrderService.Api.Infrastructure.Messaging
{
    public interface IOrderCreatedEventListenerService
    {
        Task RegisterOnMessageHandlerAndReceiveMessagesAsync();
        Task StopProcessingAsync();
    }
}