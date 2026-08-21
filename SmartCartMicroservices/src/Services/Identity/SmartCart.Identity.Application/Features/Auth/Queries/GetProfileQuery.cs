using MediatR;
using SmartCart.Identity.Application.DTOs;

namespace SmartCart.Identity.Application.Features.Auth.Queries;

public record GetProfileQuery(Guid UserId)
    : IRequest<UserProfileDto>;
