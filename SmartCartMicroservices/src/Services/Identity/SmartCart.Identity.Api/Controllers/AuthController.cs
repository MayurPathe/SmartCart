using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCart.Identity.Application.Features.Auth.Commands.Login;
using SmartCart.Identity.Application.Features.Auth.Commands.Logout;
using SmartCart.Identity.Application.Features.Auth.Commands.RefreshToken;
using SmartCart.Identity.Application.Features.Auth.Commands.Register;
using SmartCart.Identity.Application.Features.Auth.Queries;
using System.Security.Claims;

namespace SmartCart.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command =
            new LoginUserCommand(
                request.Email,
                request.Password,
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString(),
                Request.Headers.UserAgent.ToString());

        var result =
            await _sender.Send(
                command,
                cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(
        CancellationToken cancellationToken)
    {
        var userIdValue =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(
                userIdValue,
                out var userId))
        {
            return Unauthorized();
        }

        var result =
            await _sender.Send(
                new GetProfileQuery(userId),
                cancellationToken);

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                new RefreshTokenCommand(
                    request.RefreshToken),
                cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        LogoutRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new LogoutCommand(
                request.RefreshToken),
            cancellationToken);

        return Ok(
            new
            {
                message =
                    "Logout successful."
            });
    }
}

public record LoginRequest(
    string Email,
    string Password);

public record RefreshTokenRequest(
    string RefreshToken);

public record LogoutRequest(
    string RefreshToken);
