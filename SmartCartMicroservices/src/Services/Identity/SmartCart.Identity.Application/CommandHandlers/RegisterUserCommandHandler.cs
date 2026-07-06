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

public class RegisterUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<RegisterUserCommand> _validator;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IValidator<RegisterUserCommand> validator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _validator = validator;
    }

    public async Task<AuthResponseDto> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
            throw new ArgumentException(errors);
        }

        var emailExists = await _userRepository.EmailExistsAsync(command.Email, cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = command.FullName.Trim(),
            Email = command.Email.Trim().ToLowerInvariant(),
            PhoneNumber = command.PhoneNumber,
            PasswordHash = _passwordHasher.HashPassword(command.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = _jwtTokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        user.RefreshTokens.Add(refreshToken);

        await _userRepository.AddAsync(user, cancellationToken);
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
