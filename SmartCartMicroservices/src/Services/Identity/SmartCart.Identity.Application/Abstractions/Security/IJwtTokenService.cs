using SmartCart.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Application.Abstractions.Security;

public interface IJwtTokenService
{
    string GenerateAccessToken(
        User user,
        IEnumerable<string> roles);

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);

    DateTime GetAccessTokenExpiry();

    DateTime GetRefreshTokenExpiry();
}
