using SmartCart.Identity.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCart.Identity.Application.Interfaces;
using SmartCart.Identity.Domain.Entities;

namespace SmartCart.Identity.Application.Tests.Fakes;

public class FakeJwtTokenService : IJwtTokenService
{
    public string GenerateAccessToken(User user)
    {
        return $"fake-access-token-{user.Id}";
    }

    public string GenerateRefreshToken()
    {
        return "fake-refresh-token";
    }

    public DateTime GetAccessTokenExpiry()
    {
        return DateTime.UtcNow.AddMinutes(60);
    }
}
