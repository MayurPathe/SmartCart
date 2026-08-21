using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Infrastructure.Persistence.Repositories;

public class LoginAuditRepository
    : ILoginAuditRepository
{
    private readonly IdentityDbContext _dbContext;

    public LoginAuditRepository(
        IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        UserLoginAudit audit,
        CancellationToken cancellationToken)
    {
        await _dbContext.UserLoginAudits.AddAsync(
            audit,
            cancellationToken);
    }
}