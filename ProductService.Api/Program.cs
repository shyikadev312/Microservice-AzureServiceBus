using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Features.Products.Commands;
using ProductService.Api.Features.Products.Queries;
using ProductService.Api.Infrastructure.Persistance;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDatabase")));

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  // Add SwaggerGen for generating Swagger

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger JSON generation
    app.UseSwaggerUI(); // Enable Swagger UI
}

app.UseHttpsRedirection();

// Minimal API Endpoints

app.MapGet("/products", async (IMediator mediator) =>
await mediator.Send(new GetProductsQuery()));

app.MapGet("/products/{id}", async (int id, IMediator mediator) =>
    await mediator.Send(new GetProductByIdQuery(id)) is ProductService.Api.Models.Product product ? Results.Ok(product) : Results.NotFound());

app.MapPost("/products", async (CreateProductCommand command, IMediator mediator) =>
{
    var product = await mediator.Send(command);
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id}", async (int id, UpdateProductCommand command, IMediator mediator) =>
{
    command = command with { Id = id };
    await mediator.Send(command);
    return Results.NoContent();
});

app.MapDelete("/products/{id}", async (int id, IMediator mediator) =>
{
    await mediator.Send(new DeleteProductCommand(id));
    return Results.NoContent();
});

app.Run();
