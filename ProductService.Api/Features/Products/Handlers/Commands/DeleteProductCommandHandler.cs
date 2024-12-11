using MediatR;
using ProductService.Api.Features.Products.Commands;
using ProductService.Api.Infrastructure.Persistance;

namespace ProductService.Api.Features.Products.Handlers.Commands
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly ProductContext _context;

        public DeleteProductHandler(ProductContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
