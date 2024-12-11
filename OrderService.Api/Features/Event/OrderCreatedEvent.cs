namespace OrderService.Api.Features.Event
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
