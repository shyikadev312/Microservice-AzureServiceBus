using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Features.Products.Queries;
using ProductService.Api.Infrastructure.Persistance;

namespace ProductService.Api.Features.Products.Handlers.Queries
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<Models.Product>>
    {
        private readonly ProductContext _context;

        public GetProductsQueryHandler(ProductContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Products.ToListAsync(cancellationToken);
        }
    }
}
