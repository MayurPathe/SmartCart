using FluentAssertions;
using Moq;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Application.Abstractions.Security;
using SmartCart.Identity.Application.Features.Auth.Commands.Register;
using SmartCart.Identity.Domain.Entities;

namespace SmartCart.Identity.Application.Tests.Features.Auth.Commands.Register;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IPasswordService> _passwordService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IRoleRepository> _roleRepository;

    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _passwordService = new Mock<IPasswordService>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _roleRepository = new Mock<IRoleRepository>();

        _handler = new RegisterUserCommandHandler(
            _userRepository.Object,
            _roleRepository.Object,
            _passwordService.Object,
            _unitOfWork.Object
            );
    }

    [Fact]
    public async Task Handle_Should_Throw_When_UserAlreadyExists()
    {
        // Arrange

        var command = new RegisterUserCommand(
            "Mayur",
            "test@gmail.com",
            "Password123!",
            "9876543210");

        _userRepository
            .Setup(x => x.ExistsByEmailAsync(
                "TEST@GMAIL.COM",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
        .ThrowAsync<InvalidOperationException>()
        .WithMessage("A user with this email already exists.");

        _userRepository.Verify(
            x => x.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
