using AutoMapper;
using MediatR;
using SmartCart.Catalog.Application.DTOs;
using SmartCart.Catalog.Application.Interfaces;
using SmartCart.Catalog.Domain.Entities;

namespace SmartCart.Catalog.Application.Features.Categories.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public CreateProductCommandHandler(IProductRepository productRepository,
        ICategoryRepository categoryRepository, IMapper mapper, ICacheService cache)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Request.CategoryId);

        if (category == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }
        var skuExists = await _productRepository.ExistsBySkuAsync(request.Request.Sku);

        if (skuExists)
        {
            throw new InvalidOperationException("SKU alreadt Exists.");
        }

        var product = _mapper.Map<Product>(request.Request);

        product.Id = Guid.NewGuid();
        product.IsActive = true;
        product.CreatedAt = DateTime.UtcNow;

        await _productRepository.AddAsync(product);

        await _productRepository.SaveChangesAsync();

        await _cache.RemoveAsync("products:list");

        product.Category = category;

        return _mapper.Map<ProductDto>(product);
    }
}
