using MediatR;

namespace ProductService.Api.Features.Products.Queries
{
    public record GetProductsQuery() : IRequest<List<Models.Product>>;
}
