using MediatR;

namespace ProductService.Api.Features.Products.Queries
{
    public record GetProductByIdQuery(int Id) : IRequest<Models.Product>;
}
