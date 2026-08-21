using MediatR;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Application.Abstractions.Security;
using SmartCart.Identity.Application.DTOs;
using SmartCart.Identity.Domain.Constants;
using SmartCart.Identity.Domain.Entities;

namespace SmartCart.Identity.Application.Features.Auth.Commands.Register;

public class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterResponse> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail =
            request.Email.Trim().ToUpperInvariant();

        var alreadyExists =
            await _userRepository.ExistsByEmailAsync(
                normalizedEmail,
                cancellationToken);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var defaultRole =
            await _roleRepository.GetByNameAsync(
                RoleNames.User.ToUpperInvariant(),
                cancellationToken);

        if (defaultRole is null)
        {
            throw new InvalidOperationException(
                "Default User role does not exist.");
        }

        // Manual Mapping
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            NormalizedEmail = normalizedEmail,
            PhoneNumber = request.PhoneNumber.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordService.HashPassword(
                user,
                request.Password);

        user.UserRoles.Add(
            new UserRole
            {
                UserId = user.Id,
                RoleId = defaultRole.Id,
                AssignedAt = DateTime.UtcNow
            });

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new RegisterResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = defaultRole.Name
        };
    }
}