using MediatR;
using SmartCart.Catalog.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Application.Features.Categories.Commands.DeleteProduct;

public class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cache;

    public DeleteProductCommandHandler(
        IProductRepository repository,
        ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<bool> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product =
            await _repository.GetByIdAsync(request.Id);

        if (product == null)
            throw new KeyNotFoundException(
                "Product not found.");

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(product);

        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(
            $"products:{product.Id}");

        await _cache.RemoveAsync(
            "products:list");

        return true;
    }
}
