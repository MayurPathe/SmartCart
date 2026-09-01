using SmartCart.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Application.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product);

    Task<Product?> GetByIdAsync(Guid id);

    Task<bool> ExistsBySkuAsync(string sku);

    Task<List<Product>> GetProductsAsync(
        string? search,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize);

    Task UpdateAsync(Product product);

    Task SaveChangesAsync();
}
