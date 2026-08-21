using MediatR;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Application.Abstractions.Security;
using SmartCart.Identity.Application.DTOs;
using SmartCart.Identity.Domain.Entities;
using UserLoginAudit = SmartCart.Identity.Domain.Entities.UserLoginAudit;
using RefreshTokenEntity = SmartCart.Identity.Domain.Entities.RefreshToken;
using SmartCart.Identity.Application.Features.Auth.Commands.Register;

namespace SmartCart.Identity.Application.Features.Auth.Commands.Login;

public class LoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILoginAuditRepository _auditRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ILoginAuditRepository auditRepository,
        IPasswordService passwordService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _auditRepository = auditRepository;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail =
            request.Email.Trim().ToUpperInvariant();

        var user =
            await _userRepository.GetByEmailAsync(
                normalizedEmail,
                cancellationToken);

        if (user is null)
        {
            await SaveAuditAsync(
                null,
                request,
                false,
                "Invalid email or password.",
                cancellationToken);

            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            await SaveAuditAsync(
                user.Id,
                request,
                false,
                "User account is inactive.",
                cancellationToken);

            throw new UnauthorizedAccessException(
                "User account is inactive.");
        }

        var passwordValid =
            _passwordService.VerifyPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (!passwordValid)
        {
            await SaveAuditAsync(
                user.Id,
                request,
                false,
                "Invalid email or password.",
                cancellationToken);

            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var roles = user.UserRoles
            .Select(x => x.Role.Name)
            .ToList();

        var accessToken =
            _jwtTokenService.GenerateAccessToken(
                user,
                roles);

        var rawRefreshToken =
            _jwtTokenService.GenerateRefreshToken();

        var refreshTokenHash =
            _jwtTokenService.HashRefreshToken(
                rawRefreshToken);

        var refreshToken = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt =
                _jwtTokenService.GetRefreshTokenExpiry()
        };

        await _refreshTokenRepository.AddAsync(
            refreshToken,
            cancellationToken);

        await _auditRepository.AddAsync(
            new UserLoginAudit
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Email = user.Email,
                IsSuccessful = true,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                LoginAt = DateTime.UtcNow
            },
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            ExpiresAt =
                _jwtTokenService.GetAccessTokenExpiry()
        };
    }

    public object Handle(RegisterUserCommand command, CancellationToken none)
    {
        throw new NotImplementedException();
    }

    private async Task SaveAuditAsync(
        Guid? userId,
        LoginUserCommand request,
        bool isSuccessful,
        string? failureReason,
        CancellationToken cancellationToken)
    {
        await _auditRepository.AddAsync(
            new UserLoginAudit
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Email = request.Email,
                IsSuccessful = isSuccessful,
                FailureReason = failureReason,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                LoginAt = DateTime.UtcNow
            },
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
