using Microsoft.EntityFrameworkCore;
using OrderService.Api.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the IOrderCreatedEventSenderService as a singleton
builder.Services.AddSingleton<IOrderCreatedEventSenderService, OrderCreatedEventSenderService>();

builder.Services.AddDbContext<OrderContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDatabase")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register application shutdown event to clean up the service bus resources
var lifetime = app.Lifetime;
var eventSenderService = app.Services.GetRequiredService<IOrderCreatedEventSenderService>();

lifetime.ApplicationStopping.Register(async () =>
{
    // Ensure that the Service Bus sender and client are properly disposed of on shutdown
    await eventSenderService.CloseQueueAsync();
});

// Map CRUD endpoints for Orders
app.MapGet("/orders", async (OrderContext db) => await db.Orders.ToListAsync());
app.MapGet("/orders/{id}", async (int id, OrderContext db) =>
    await db.Orders.FindAsync(id) is Order order ? Results.Ok(order) : Results.NotFound());

app.MapPost("/orders", async (Order order, OrderContext db, IOrderCreatedEventSenderService orderService) =>
{
    // Save the order to the database
    db.Orders.Add(order);
    await db.SaveChangesAsync();

    // Publish the event to the Service Bus via the injected service
    await orderService.PublishOrderCreatedEvent(order);

    return Results.Created($"/orders/{order.Id}", order);
});

app.MapPut("/orders/{id}", async (int id, Order inputOrder, OrderContext db) =>
{
    var order = await db.Orders.FindAsync(id);

    if (order is null) return Results.NotFound();

    order.TotalPrice = inputOrder.TotalPrice;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/orders/{id}", async (int id, OrderContext db) =>
{
    if (await db.Orders.FindAsync(id) is Order order)
    {
        db.Orders.Remove(order);
        await db.SaveChangesAsync();
        return Results.Ok(order);
    }

    return Results.NotFound();
});

app.Run();

// Order entity
public record Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal TotalPrice { get; set; }
};

// DbContext for Order service
class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
}
