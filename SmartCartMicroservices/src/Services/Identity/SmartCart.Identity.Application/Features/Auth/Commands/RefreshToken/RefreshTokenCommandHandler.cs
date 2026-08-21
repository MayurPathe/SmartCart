using MediatR;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Application.Abstractions.Security;
using SmartCart.Identity.Application.DTOs;
using SmartCart.Identity.Domain.Entities;
using RefreshTokenEntity = SmartCart.Identity.Domain.Entities.RefreshToken;
namespace SmartCart.Identity.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            _jwtTokenService.HashRefreshToken(
                request.RefreshToken);

        var existingToken =
            await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        if (existingToken is null ||
            !existingToken.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Invalid or expired refresh token.");
        }

        var user = existingToken.User;

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "User account is inactive.");
        }

        var roles = user.UserRoles
            .Select(x => x.Role.Name)
            .ToList();

        var newAccessToken =
            _jwtTokenService.GenerateAccessToken(
                user,
                roles);

        var newRawRefreshToken =
            _jwtTokenService.GenerateRefreshToken();

        var newRefreshTokenHash =
            _jwtTokenService.HashRefreshToken(
                newRawRefreshToken);

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByTokenHash =
            newRefreshTokenHash;

        await _refreshTokenRepository.AddAsync(
            new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newRefreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt =
                    _jwtTokenService.GetRefreshTokenExpiry()
            },
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRawRefreshToken,
            ExpiresAt =
                _jwtTokenService.GetAccessTokenExpiry()
        };
    }
}