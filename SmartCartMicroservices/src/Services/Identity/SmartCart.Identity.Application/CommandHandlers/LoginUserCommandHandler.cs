using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartCart.Identity.Application.Commands;
using SmartCart.Identity.Application.DTOs;
using SmartCart.Identity.Application.Interfaces;
using SmartCart.Identity.Domain.Entities;

namespace SmartCart.Identity.Application.CommandHandlers;

public class LoginUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<LoginUserCommand> _validator;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IValidator<LoginUserCommand> validator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _validator = validator;
    }

    public async Task<AuthResponseDto> HandleAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
            throw new ArgumentException(errors);
        }
        var normalizedEmail =
            command.Email.Trim().ToLowerInvariant();


        var user = await _userRepository.GetByEmailAsync(
            command.Email.Trim().ToLowerInvariant(),
            cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is inactive.");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(command.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = _jwtTokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        //user.RefreshTokens.Add(refreshToken);
        // 7. Explicitly INSERT refresh token
        await _userRepository.AddRefreshTokenAsync(
            refreshToken,
            cancellationToken);

        await _userRepository.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            AccessToken = _jwtTokenService.GenerateAccessToken(user),
            RefreshToken = refreshToken.Token,
            ExpiresAt = _jwtTokenService.GetAccessTokenExpiry()
        };
    }
}
