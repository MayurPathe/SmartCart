using AutoMapper;
using MediatR;
using SmartCart.Catalog.Application.DTOs;
using SmartCart.Catalog.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Application.Features.Categories.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetProductsQueryHandler(
        IProductRepository repository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _repository = repository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<List<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey =
            $"products:list:{request.Request.Page}:" +
            $"{request.Request.PageSize}:" +
            $"{request.Request.Search}:" +
            $"{request.Request.CategoryId}:" +
            $"{request.Request.MinPrice}:" +
            $"{request.Request.MaxPrice}";

        var cached =
            await _cacheService.GetAsync<List<ProductDto>>(cacheKey);

        if (cached != null)
            return cached;

        var products =
            await _repository.GetProductsAsync(
                request.Request.Search,
                request.Request.CategoryId,
                request.Request.MinPrice,
                request.Request.MaxPrice,
                request.Request.Page,
                request.Request.PageSize);

        var result =
            _mapper.Map<List<ProductDto>>(products);

        await _cacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(5));

        return result;
    }
}
