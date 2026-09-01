using AutoMapper;
using MediatR;
using SmartCart.Catalog.Application.DTOs;
using SmartCart.Catalog.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Application.Features.Categories.Queries.GetProductById;

public class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public GetProductByIdQueryHandler(
        IProductRepository repository,
        IMapper mapper,
        ICacheService cache)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ProductDto> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey =
            $"products:{request.Id}";

        var cached =
            await _cache.GetAsync<ProductDto>(cacheKey);

        if (cached != null)
            return cached;

        var product =
            await _repository.GetByIdAsync(request.Id);

        if (product == null || !product.IsActive)
            throw new KeyNotFoundException(
                "Product not found.");

        var result =
            _mapper.Map<ProductDto>(product);

        await _cache.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(10));

        return result;
    }
}
