using Microsoft.AspNetCore.Identity;
using SmartCart.Identity.Application.Abstractions.Security;
using SmartCart.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Infrastructure.Security;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _passwordHasher
        = new();

    public string HashPassword(
        User user,
        string password)
    {
        return _passwordHasher.HashPassword(
            user,
            password);
    }

    public bool VerifyPassword(
        User user,
        string hashedPassword,
        string suppliedPassword)
    {
        var result =
            _passwordHasher.VerifyHashedPassword(
                user,
                hashedPassword,
                suppliedPassword);

        return result is
            PasswordVerificationResult.Success or
            PasswordVerificationResult.SuccessRehashNeeded;
    }
}
