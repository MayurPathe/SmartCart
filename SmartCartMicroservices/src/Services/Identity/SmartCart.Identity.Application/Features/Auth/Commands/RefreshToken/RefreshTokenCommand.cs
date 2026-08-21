using MediatR;
using SmartCart.Identity.Application.DTOs;

namespace SmartCart.Identity.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken)
    : IRequest<AuthResponse>;