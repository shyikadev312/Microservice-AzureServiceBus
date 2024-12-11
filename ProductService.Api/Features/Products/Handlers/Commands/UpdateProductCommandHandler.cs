using MediatR;
using ProductService.Api.Features.Products.Commands;
using ProductService.Api.Infrastructure.Persistance;

namespace ProductService.Api.Features.Products.Handlers.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly ProductContext _context;

        public UpdateProductCommandHandler(ProductContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(request.Id);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            product.Name = request.Name;
            product.Price = request.Price;

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
