using Microsoft.EntityFrameworkCore;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly IdentityDbContext _dbContext;

    public RoleRepository(
        IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetByNameAsync(
        string normalizedRoleName,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Roles
            .FirstOrDefaultAsync(
                x =>
                    x.NormalizedName ==
                    normalizedRoleName,
                cancellationToken);
    }
}