using MediatR;
using ProductService.Api.Features.Products.Queries;
using ProductService.Api.Infrastructure.Persistance;

namespace ProductService.Api.Features.Products.Handlers.Queries
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Models.Product>
    {
        private readonly ProductContext _context;

        public GetProductByIdHandler(ProductContext context)
        {
            _context = context;
        }

        public async Task<Models.Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
        }
    }
}
