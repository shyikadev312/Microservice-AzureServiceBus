using MediatR;

namespace ProductService.Api.Features.Products.Commands
{
    public record UpdateProductCommand(int Id, string Name, decimal Price) : IRequest<Unit>;
}
