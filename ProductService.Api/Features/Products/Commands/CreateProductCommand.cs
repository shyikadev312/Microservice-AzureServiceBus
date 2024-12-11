using MediatR;

namespace ProductService.Api.Features.Products.Commands
{
    public record CreateProductCommand(string Name, decimal Price) : IRequest<Models.Product>;
}
