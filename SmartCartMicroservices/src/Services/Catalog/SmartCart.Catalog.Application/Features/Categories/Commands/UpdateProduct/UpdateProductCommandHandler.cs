using AutoMapper;
using MediatR;
using SmartCart.Catalog.Application.DTOs;
using SmartCart.Catalog.Application.Interfaces;
using SmartCart.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Application.Features.Categories.Commands.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ICacheService cache)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ProductDto> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product =
            await _productRepository.GetByIdAsync(request.Id);

        if (product == null)
            throw new KeyNotFoundException(
                "Product not found.");

        var category =
            await _categoryRepository.GetByIdAsync(
                request.Request.CategoryId);

        if (category == null)
            throw new KeyNotFoundException(
                "Category not found.");

        var oldPrice = product.Price;

        _mapper.Map(request.Request, product);

        product.UpdatedAt = DateTime.UtcNow;

        if (oldPrice != product.Price)
        {
            var history = new ProductPriceHistory
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                OldPrice = oldPrice,
                NewPrice = product.Price,
                ChangedAt = DateTime.UtcNow
            };

            product.PriceHistories.Add(history);
        }

        await _productRepository.UpdateAsync(product);

        await _productRepository.SaveChangesAsync();

        await _cache.RemoveAsync(
            $"products:{product.Id}");

        await _cache.RemoveAsync(
            "products:list");

        product.Category = category;

        return _mapper.Map<ProductDto>(product);
    }
}