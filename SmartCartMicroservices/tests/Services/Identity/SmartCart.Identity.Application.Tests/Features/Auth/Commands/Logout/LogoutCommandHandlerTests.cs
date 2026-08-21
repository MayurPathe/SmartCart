using FluentAssertions;
using Moq;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Application.Abstractions.Security;
using SmartCart.Identity.Application.Features.Auth.Commands.Logout;
using RefreshTokenEntity = SmartCart.Identity.Domain.Entities.RefreshToken;

namespace SmartCart.Identity.Application.Tests.Features.Auth.Commands.Logout;

public class LogoutCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository;
    private readonly Mock<IJwtTokenService> _jwtTokenService;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _refreshTokenRepository =
            new Mock<IRefreshTokenRepository>();

        _jwtTokenService =
            new Mock<IJwtTokenService>();

        _unitOfWork =
            new Mock<IUnitOfWork>();

        _handler =
            new LogoutCommandHandler(
                _refreshTokenRepository.Object,
                _jwtTokenService.Object,
                _unitOfWork.Object);
    }


    // ============================================================
    // TEST 1
    // Refresh token does not exist
    // ============================================================

    [Fact]
    public async Task Handle_Should_ReturnWithoutSaving_When_RefreshTokenDoesNotExist()
    {
        // Arrange

        var command =
            new LogoutCommand(
                "invalid-refresh-token");

        _jwtTokenService
            .Setup(x => x.HashRefreshToken(
                "invalid-refresh-token"))
            .Returns("hashed-token");

        _refreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(
                "hashed-token",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (RefreshTokenEntity?)null);

        // Act

        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        _jwtTokenService.Verify(
            x => x.HashRefreshToken(
                "invalid-refresh-token"),
            Times.Once);

        _refreshTokenRepository.Verify(
            x => x.GetByTokenHashAsync(
                "hashed-token",
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    // ============================================================
    // TEST 2
    // Refresh token exists and is not revoked
    // ============================================================

    [Fact]
    public async Task Handle_Should_RevokeToken_When_RefreshTokenExists()
    {
        // Arrange

        var token = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TokenHash = "hashed-token",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        var command =
            new LogoutCommand(
                "valid-refresh-token");

        _jwtTokenService
            .Setup(x => x.HashRefreshToken(
                "valid-refresh-token"))
            .Returns("hashed-token");

        _refreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(
                "hashed-token",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        // Act

        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        token.RevokedAt
            .Should()
            .NotBeNull();

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    // ============================================================
    // TEST 3
    // Refresh token is already revoked
    // ============================================================

    [Fact]
    public async Task Handle_Should_NotSaveChanges_When_TokenIsAlreadyRevoked()
    {
        // Arrange

        var token = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TokenHash = "hashed-token",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = DateTime.UtcNow.AddHours(-1)
        };

        var originalRevokedAt =
            token.RevokedAt;

        var command =
            new LogoutCommand(
                "already-revoked-token");

        _jwtTokenService
            .Setup(x => x.HashRefreshToken(
                "already-revoked-token"))
            .Returns("hashed-token");

        _refreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(
                "hashed-token",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        // Act

        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        token.RevokedAt
            .Should()
            .Be(originalRevokedAt);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    // ============================================================
    // TEST 4
    // Verify correct token hash is used
    // ============================================================

    [Fact]
    public async Task Handle_Should_HashRefreshToken_BeforeSearchingRepository()
    {
        // Arrange

        var command =
            new LogoutCommand(
                "my-refresh-token");

        _jwtTokenService
            .Setup(x => x.HashRefreshToken(
                "my-refresh-token"))
            .Returns("my-hashed-token");

        _refreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(
                "my-hashed-token",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (RefreshTokenEntity?)null);

        // Act

        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        _jwtTokenService.Verify(
            x => x.HashRefreshToken(
                "my-refresh-token"),
            Times.Once);

        _refreshTokenRepository.Verify(
            x => x.GetByTokenHashAsync(
                "my-hashed-token",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
