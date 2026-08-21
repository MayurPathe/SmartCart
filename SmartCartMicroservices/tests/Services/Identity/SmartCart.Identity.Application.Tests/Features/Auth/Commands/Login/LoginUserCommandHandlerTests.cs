using FluentAssertions;
using Moq;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Application.Abstractions.Security;
using SmartCart.Identity.Application.Features.Auth.Commands.Login;
using SmartCart.Identity.Domain.Entities;
using RefreshTokenEntity = SmartCart.Identity.Domain.Entities.RefreshToken;

namespace SmartCart.Identity.Application.Tests.Features.Auth.Commands.Login;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository;
    private readonly Mock<ILoginAuditRepository> _auditRepository;
    private readonly Mock<IPasswordService> _passwordService;
    private readonly Mock<IJwtTokenService> _jwtTokenService;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        _auditRepository = new Mock<ILoginAuditRepository>();
        _passwordService = new Mock<IPasswordService>();
        _jwtTokenService = new Mock<IJwtTokenService>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new LoginUserCommandHandler(
            _userRepository.Object,
            _refreshTokenRepository.Object,
            _auditRepository.Object,
            _passwordService.Object,
            _jwtTokenService.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_UserDoesNotExist()
    {
        // Arrange

        var command = new LoginUserCommand(
            "test@gmail.com",
            "Password123!",
            "127.0.0.1",
            "TestAgent");

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                "TEST@GMAIL.COM",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act

        var act = () =>
            _handler.Handle(
                command,
                CancellationToken.None);

        // Assert

        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password.");

        _auditRepository.Verify(
            x => x.AddAsync(
                It.IsAny<UserLoginAudit>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
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

        var command = new LoginUserCommand(
            "test@gmail.com",
            "Password123!",
            "127.0.0.1",
            "TestAgent");

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                "TEST@GMAIL.COM",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act

        var act = () =>
            _handler.Handle(
                command,
                CancellationToken.None);

        // Assert

        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User account is inactive.");

        _auditRepository.Verify(
            x => x.AddAsync(
                It.IsAny<UserLoginAudit>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorized_When_PasswordIsInvalid()
    {
        // Arrange

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@gmail.com",
            NormalizedEmail = "TEST@GMAIL.COM",
            IsActive = true
        };

        var command = new LoginUserCommand(
            "test@gmail.com",
            "WrongPassword!",
            null,
            null);

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                "TEST@GMAIL.COM",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordService
            .Setup(x => x.VerifyPassword(
                user,
                user.PasswordHash,
                command.Password))
            .Returns(false);

        // Act

        var act = () =>
            _handler.Handle(
                command,
                CancellationToken.None);

        // Assert

        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password.");

        _auditRepository.Verify(
            x => x.AddAsync(
                It.Is<UserLoginAudit>(
                    a => !a.IsSuccessful &&
                         a.FailureReason ==
                         "Invalid email or password."),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnAuthResponse_When_LoginIsSuccessful()
    {
        // Arrange

        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = "test@gmail.com",
            NormalizedEmail = "TEST@GMAIL.COM",
            PasswordHash = "HASH",
            IsActive = true
        };

        var command = new LoginUserCommand(
            "test@gmail.com",
            "Password123!",
            "127.0.0.1",
            "TestAgent");

        _userRepository
            .Setup(x => x.GetByEmailAsync(
                "TEST@GMAIL.COM",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordService
            .Setup(x => x.VerifyPassword(
                user,
                user.PasswordHash,
                command.Password))
            .Returns(true);

        _jwtTokenService
            .Setup(x => x.GenerateAccessToken(
                user,
                It.IsAny<List<string>>()))
            .Returns("access-token");

        _jwtTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        _jwtTokenService
            .Setup(x => x.HashRefreshToken("refresh-token"))
            .Returns("refresh-token-hash");

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
            .Be("access-token");

        result.RefreshToken
            .Should()
            .Be("refresh-token");

        _refreshTokenRepository.Verify(
            x => x.AddAsync(
                It.Is<RefreshTokenEntity>(
                    r => r.UserId == userId &&
                         r.TokenHash == "refresh-token-hash"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _auditRepository.Verify(
            x => x.AddAsync(
                It.Is<UserLoginAudit>(
                    a => a.UserId == userId &&
                         a.IsSuccessful),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}