using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCart.Identity.Application.Commands;
using SmartCart.Identity.Application.CommandHandlers;
using SmartCart.Identity.Application.DTOs;
using SmartCart.Identity.Application.Queries;
using SmartCart.Identity.Application.QueryHandlers;

namespace SmartCart.Identity.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly RegisterUserCommandHandler _registerUserCommandHandler;
    private readonly LoginUserCommandHandler _loginUserCommandHandler;
    private readonly GetUserProfileQueryHandler _getUserProfileQueryHandler;
    private readonly RefreshTokenCommandHandler _refreshTokenCommandHandler;
    private readonly LogoutCommandHandler _logoutCommandHandler;

    public AuthController(
       RegisterUserCommandHandler registerUserCommandHandler,
       LoginUserCommandHandler loginUserCommandHandler,
       GetUserProfileQueryHandler getUserProfileQueryHandler,
        LogoutCommandHandler logoutCommandHandler,
        RefreshTokenCommandHandler refreshTokenCommandHandler)

    {
        _registerUserCommandHandler = registerUserCommandHandler;
        _loginUserCommandHandler = loginUserCommandHandler;
        _getUserProfileQueryHandler = getUserProfileQueryHandler;
        _refreshTokenCommandHandler = refreshTokenCommandHandler;
        _logoutCommandHandler = logoutCommandHandler;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        var command = new RegisterUserCommand
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _registerUserCommandHandler.HandleAsync(command);

        return Ok(result);
    }
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        var command = new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _loginUserCommandHandler.HandleAsync(command);

        return Ok(result);
    }
    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> Profile()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdValue))
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdValue);

        var result = await _getUserProfileQueryHandler.HandleAsync(
            new GetUserProfileQuery(userId));

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(
    RefreshTokenRequestDto request)
    {
        var command = new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken
        };

        var result =
            await _refreshTokenCommandHandler.HandleAsync(command);

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    LogoutRequestDto request)
    {
        var command = new LogoutCommand
        {
            RefreshToken = request.RefreshToken
        };

        await _logoutCommandHandler.HandleAsync(command);

        return Ok(new
        {
            message = "Logout successful."
        });
    }
}
