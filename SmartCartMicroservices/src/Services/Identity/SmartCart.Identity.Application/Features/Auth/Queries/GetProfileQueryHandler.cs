using AutoMapper;
using MediatR;
using SmartCart.Identity.Application.Abstractions.Persistence;
using SmartCart.Identity.Application.DTOs;

namespace SmartCart.Identity.Application.Features.Auth.Queries;

public class GetProfileQueryHandler
    : IRequestHandler<GetProfileQuery, UserProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetProfileQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository.GetByIdWithRolesAsync(
                request.UserId,
                cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User was not found.");
        }

        return _mapper.Map<UserProfileDto>(user);
    }
}