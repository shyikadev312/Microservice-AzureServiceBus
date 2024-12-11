using Azure.Messaging.ServiceBus;
using PaymentService.Api.Feature.Event;
using System.Text.Json;

namespace OrderService.Api.Infrastructure.Messaging
{
    public class OrderCreatedEventListenerService : IOrderCreatedEventListenerService
    {
        private readonly string connectionString = "YourServiceBusConnectionString";
        private readonly string queueName = "orders";
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceProvider _serviceProvider; // Inject IServiceProvider to resolve scoped services

        public OrderCreatedEventListenerService(IServiceProvider serviceProvider)
        {
            _client = new ServiceBusClient(connectionString);
            _processor = _client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
            _serviceProvider = serviceProvider;
        }

        public async Task RegisterOnMessageHandlerAndReceiveMessagesAsync()
        {
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            // Start processing asynchronously
            await _processor.StartProcessingAsync();
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

            // Use IServiceProvider to create a scope and resolve PaymentContext
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentContext>();

                // Create a new payment entry based on the order event
                var payment = new Payment
                {
                    OrderId = orderCreatedEvent.OrderId,
                    Amount = orderCreatedEvent.TotalPrice // Assuming TotalPrice is the payment amount
                };

                // Insert the payment record into the database
                dbContext.Payments.Add(payment);
                await dbContext.SaveChangesAsync();
            }

            // Mark the message as complete to acknowledge it
            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Message handler encountered an error: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        public async Task StopProcessingAsync()
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}
