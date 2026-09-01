using Microsoft.EntityFrameworkCore;
using SmartCart.Catalog.Application.Interfaces;
using SmartCart.Catalog.Domain.Entities;
using SmartCart.Catalog.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _context;

    public ProductRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(x => x.Category)
            .Include(x => x.PriceHistories)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsBySkuAsync(string sku)
    {
        return await _context.Products
            .AnyAsync(x => x.Sku == sku);
    }

    public async Task<List<Product>> GetProductsAsync(
        string? search,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Name.ToLower()
                    .Contains(search.ToLower()));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x =>
                x.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(x =>
                x.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(x =>
                x.Price <= maxPrice.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
