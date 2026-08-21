using AutoMapper;
using FluentAssertions;
using Moq;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Application.DTOs;
using SmartCart.Identity.Application.Features.Auth.Queries;
using SmartCart.Identity.Domain.Entities;

namespace SmartCart.Identity.Application.Tests.Features.Auth.Queries;

public class GetProfileQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IMapper> _mapper;

    private readonly GetProfileQueryHandler _handler;

    public GetProfileQueryHandlerTests()
    {
        _userRepository =
            new Mock<IUserRepository>();

        _mapper =
            new Mock<IMapper>();

        _handler =
            new GetProfileQueryHandler(
                _userRepository.Object,
                _mapper.Object);
    }


    // ============================================================
    // TEST 1
    // User does not exist
    // ============================================================

    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_When_UserDoesNotExist()
    {
        // Arrange

        var userId =
            Guid.NewGuid();

        var query =
            new GetProfileQuery(userId);

        _userRepository
            .Setup(x => x.GetByIdWithRolesAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (User?)null);

        // Act

        var act = () =>
            _handler.Handle(
                query,
                CancellationToken.None);

        // Assert

        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage(
                "User was not found.");

        _userRepository.Verify(
            x => x.GetByIdWithRolesAsync(
                userId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapper.Verify(
            x => x.Map<UserProfileDto>(
                It.IsAny<User>()),
            Times.Never);
    }


    // ============================================================
    // TEST 2
    // User exists
    // ============================================================

    [Fact]
    public async Task Handle_Should_ReturnUserProfile_When_UserExists()
    {
        // Arrange

        var userId =
            Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            FullName = "Mayur",
            Email = "mayur@gmail.com",
            NormalizedEmail = "MAYUR@GMAIL.COM",
            PhoneNumber = "9876543210",
            IsActive = true
        };

        var expectedProfile =
            new UserProfileDto
            {
                UserId = userId,
                FullName = "Mayur",
                Email = "mayur@gmail.com",
                PhoneNumber = "9876543210"
            };

        var query =
            new GetProfileQuery(userId);

        _userRepository
            .Setup(x => x.GetByIdWithRolesAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapper
            .Setup(x => x.Map<UserProfileDto>(
                user))
            .Returns(expectedProfile);

        // Act

        var result =
            await _handler.Handle(
                query,
                CancellationToken.None);

        // Assert

        result.Should()
            .NotBeNull();

        result.Should()
            .BeEquivalentTo(expectedProfile);

        _userRepository.Verify(
            x => x.GetByIdWithRolesAsync(
                userId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapper.Verify(
            x => x.Map<UserProfileDto>(
                user),
            Times.Once);
    }


    // ============================================================
    // TEST 3
    // Verify correct UserId is passed
    // ============================================================

    [Fact]
    public async Task Handle_Should_RequestCorrectUserId()
    {
        // Arrange

        var userId =
            Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            FullName = "Mayur",
            Email = "mayur@gmail.com",
            NormalizedEmail = "MAYUR@GMAIL.COM",
            PhoneNumber = "9876543210",
            IsActive = true
        };

        var profile =
            new UserProfileDto
            {
                UserId = userId,
                FullName = "Mayur",
                Email = "mayur@gmail.com",
                PhoneNumber = "9876543210"
            };

        var query =
            new GetProfileQuery(userId);

        _userRepository
            .Setup(x => x.GetByIdWithRolesAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapper
            .Setup(x => x.Map<UserProfileDto>(
                user))
            .Returns(profile);

        // Act

        await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert

        _userRepository.Verify(
            x => x.GetByIdWithRolesAsync(
                userId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    // ============================================================
    // TEST 4
    // Mapper should be called only when user exists
    // ============================================================

    [Fact]
    public async Task Handle_Should_MapUser_When_UserExists()
    {
        // Arrange

        var userId =
            Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            FullName = "Mayur",
            Email = "mayur@gmail.com",
            NormalizedEmail = "MAYUR@GMAIL.COM",
            PhoneNumber = "9876543210",
            IsActive = true
        };

        var profile =
            new UserProfileDto
            {
                UserId = userId,
                FullName = "Mayur",
                Email = "mayur@gmail.com",
                PhoneNumber = "9876543210"
            };

        var query =
            new GetProfileQuery(userId);

        _userRepository
            .Setup(x => x.GetByIdWithRolesAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapper
            .Setup(x => x.Map<UserProfileDto>(
                user))
            .Returns(profile);

        // Act

        var result =
            await _handler.Handle(
                query,
                CancellationToken.None);

        // Assert

        _mapper.Verify(
            x => x.Map<UserProfileDto>(
                user),
            Times.Once);

        result.Should()
            .BeEquivalentTo(profile);
    }
}