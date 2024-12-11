using Microsoft.EntityFrameworkCore;
using OrderService.Api.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the DbContext with SQL Server for Payments
builder.Services.AddDbContext<PaymentContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDatabase")));

builder.Services.AddSingleton<IOrderCreatedEventListenerService, OrderCreatedEventListenerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Start the listener when the application starts
var eventListener = app.Services.GetRequiredService<IOrderCreatedEventListenerService>();

// Make the context asynchronous by wrapping it in an async method.
async Task StartMessageListener()
{
    await eventListener.RegisterOnMessageHandlerAndReceiveMessagesAsync();
}

// Run the task asynchronously when the application starts
await StartMessageListener();

// Map CRUD endpoints for Payments

// Get all payments
app.MapGet("/payments", async (PaymentContext db) => await db.Payments.ToListAsync());

// Get a payment by id
app.MapGet("/payments/{id}", async (int id, PaymentContext db) =>
    await db.Payments.FindAsync(id) is Payment payment ? Results.Ok(payment) : Results.NotFound());

// Create a new payment
app.MapPost("/payments", async (Payment payment, PaymentContext db) =>
{
    db.Payments.Add(payment);
    await db.SaveChangesAsync();
    return Results.Created($"/payments/{payment.Id}", payment);
});

// Update an existing payment
app.MapPut("/payments/{id}", async (int id, Payment inputPayment, PaymentContext db) =>
{
    var payment = await db.Payments.FindAsync(id);

    if (payment is null) return Results.NotFound();

    payment.Amount = inputPayment.Amount;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Delete a payment
app.MapDelete("/payments/{id}", async (int id, PaymentContext db) =>
{
    if (await db.Payments.FindAsync(id) is Payment payment)
    {
        db.Payments.Remove(payment);
        await db.SaveChangesAsync();
        return Results.Ok(payment);
    }

    return Results.NotFound();
});

app.Run();

// Payment entity
public record Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
};

// DbContext for Payment service
public class PaymentContext : DbContext
{
    public PaymentContext(DbContextOptions<PaymentContext> options) : base(options) { }

    public DbSet<Payment> Payments { get; set; }
}
