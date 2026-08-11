using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartCart.Identity.Application.Interfaces;
using SmartCart.Identity.Domain.Entities;
using SmartCart.Identity.Infrastructure.Data;

namespace SmartCart.Identity.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _dbContext;

    public UserRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        //return await _dbContext.Users
        //    .Include(x => x.RefreshTokens)
        //    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return await _dbContext.Users
           .AsNoTracking()
           .FirstOrDefaultAsync(
               x => x.Id == id,
               cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        //return await _dbContext.Users
        //    .Include(x => x.RefreshTokens)
        //    .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        return await _dbContext.Users
           .AsNoTracking()
           .FirstOrDefaultAsync(
               x => x.Email == email,
               cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(x => x.Email == email.Trim().ToLowerInvariant(), cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }
    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token,CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.Token == token,
                cancellationToken);
    }
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
