using MediatR;
using ProductService.Api.Features.Products.Commands;
using ProductService.Api.Infrastructure.Persistance;

namespace ProductService.Api.Features.Products.Handlers.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Models.Product>
    {
        private readonly ProductContext _context;

        public CreateProductCommandHandler(ProductContext context)
        {
            _context = context;
        }

        public async Task<Models.Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Models.Product { Name = request.Name, Price = request.Price };
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);
            return product;
        }
    }
}
