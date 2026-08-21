using SmartCart.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Application.Abstractions.Persistence;

public interface ILoginAuditRepository
{
    Task AddAsync(
        UserLoginAudit audit,
        CancellationToken cancellationToken);
}
