using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCart.Identity.Application.Interfaces;

namespace SmartCart.Identity.Application.Tests.Fakes;

public class FakePasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return $"HASHED_{password}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return passwordHash == $"HASHED_{password}";
    }
}
