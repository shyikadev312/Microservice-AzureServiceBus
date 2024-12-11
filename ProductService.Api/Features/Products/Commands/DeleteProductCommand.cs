using MediatR;

namespace ProductService.Api.Features.Products.Commands
{
    public record DeleteProductCommand(int Id) : IRequest<Unit>;
}
