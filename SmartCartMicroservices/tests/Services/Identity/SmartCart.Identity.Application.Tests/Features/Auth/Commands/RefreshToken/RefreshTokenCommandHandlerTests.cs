using FluentAssertions;
using Moq;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Application.Abstractions.Security;
using SmartCart.Identity.Application.Features.Auth.Commands.RefreshToken;
using SmartCart.Identity.Domain.Entities;
using RefreshTokenEntity = SmartCart.Identity.Domain.Entities.RefreshToken;

namespace SmartCart.Identity.Application.Tests.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository;
    private readonly Mock<IJwtTokenService> _jwtTokenService;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        _jwtTokenService = new Mock<IJwtTokenService>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new RefreshTokenCommandHandler(
            _refreshTokenRepository.Object,
            _jwtTokenService.Object,
            _unitOfWork.Object
            );

    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_RefreshTokenDoesNotExist()
    {
        // Arrange

        var command =
            new RefreshTokenCommand("invalid-refresh-token");

        _jwtTokenService
            .Setup(x => x.HashRefreshToken(
                "invalid-refresh-token"))
            .Returns("hashed-token");

        _refreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(
                "hashed-token",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshTokenEntity?)null);

        // Act

        var act = () =>
            _handler.Handle(
                command,
                CancellationToken.None);

        // Assert

        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage(
                "Invalid or expired refresh token.");

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_RefreshTokenIsInactive()
    {
        // Arrange

        var token = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TokenHash = "hashed-token",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };

        var command =
            new RefreshTokenCommand("expired-refresh-token");

        _jwtTokenService
            .Setup(x => x.HashRefreshToken(
                "expired-refresh-token"))
            .Returns("hashed-token");

        _refreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(
                "hashed-token",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        // Act

        var act = () =>
            _handler.Handle(
                command,
                CancellationToken.None);

        // Assert

        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage(
                "Invalid or expired refresh token.");

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_UserIsInactive()
    {
        // Arrange

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@gmail.com",
            NormalizedEmail = "TEST@GMAIL.COM",
            IsActive = false
        };

        var token = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = "hashed-token",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = user
        };

        var command =
            new RefreshTokenCommand("refresh-token");

        _jwtTokenService
            .Setup(x => x.HashRefreshToken(
                "refresh-token"))
            .Returns("hashed-token");

        _refreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(
                "hashed-token",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        // Act

        var act = () =>
            _handler.Handle(
                command,
                CancellationToken.None);

        // Assert

        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage(
                "User account is inactive.");

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    [Fact]
    public async Task Handle_Should_ReturnNewTokens_When_RefreshTokenIsValid()
    {
        // Arrange

        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = "test@gmail.com",
            NormalizedEmail = "TEST@GMAIL.COM",
            IsActive = true
        };

        var existingToken = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = "old-hash",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = user
        };

        var command =
            new RefreshTokenCommand(
                "old-refresh-token");

        _jwtTokenService
            .Setup(x => x.HashRefreshToken(
                "old-refresh-token"))
            .Returns("old-hash");

        _refreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(
                "old-hash",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingToken);

        _jwtTokenService
            .Setup(x => x.GenerateAccessToken(
                user,
                It.IsAny<List<string>>()))
            .Returns("new-access-token");

        _jwtTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");

        _jwtTokenService
            .Setup(x => x.HashRefreshToken(
                "new-refresh-token"))
            .Returns("new-refresh-hash");

        _jwtTokenService
            .Setup(x => x.GetRefreshTokenExpiry())
            .Returns(DateTime.UtcNow.AddDays(7));

        _jwtTokenService
            .Setup(x => x.GetAccessTokenExpiry())
            .Returns(DateTime.UtcNow.AddMinutes(15));

        // Act

        var result =
            await _handler.Handle(
                command,
                CancellationToken.None);

        // Assert

        result.Should().NotBeNull();

        result.AccessToken
            .Should()
            .Be("new-access-token");

        result.RefreshToken
            .Should()
            .Be("new-refresh-token");

        // Old token should be revoked
        existingToken.RevokedAt
            .Should()
            .NotBeNull();

        existingToken.ReplacedByTokenHash
            .Should()
            .Be("new-refresh-hash");

        // New refresh token should be inserted
        _refreshTokenRepository.Verify(
            x => x.AddAsync(
                It.Is<RefreshTokenEntity>(
                    token =>
                        token.UserId == userId &&
                        token.TokenHash == "new-refresh-hash"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        // Database should be saved
        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


}
