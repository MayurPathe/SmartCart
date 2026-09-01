using SmartCart.Catalog.Domain.Entities;

namespace SmartCart.Catalog.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);

    Task<List<Category>> GetAllAsync();

    Task AddAsync(Category category);

    Task<bool> ExistsByNameAsync(string name);

    Task SaveChangesAsync();
}
