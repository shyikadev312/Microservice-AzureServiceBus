using Azure.Messaging.ServiceBus;
using OrderService.Api.Features.Event;
using System.Text.Json;

namespace OrderService.Api.Infrastructure.Messaging
{
    public class OrderCreatedEventSenderService : IOrderCreatedEventSenderService
    {
        private readonly string connectionString = "YourServiceBusConnectionString";
        private readonly string queueName = "orders";
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public OrderCreatedEventSenderService()
        {
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(queueName);
        }

        public async Task PublishOrderCreatedEvent(Order order)
        {
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice
            };

            var messageBody = JsonSerializer.Serialize(orderCreatedEvent);
            var message = new ServiceBusMessage(messageBody);

            await _sender.SendMessageAsync(message);
        }

        // Close client when done
        public async Task CloseQueueAsync()
        {
            await _sender.DisposeAsync();
            await _client.DisposeAsync();
        }

    }
}
