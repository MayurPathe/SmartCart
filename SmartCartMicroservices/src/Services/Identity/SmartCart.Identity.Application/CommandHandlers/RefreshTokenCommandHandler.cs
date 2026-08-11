using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCart.Identity.Application.Commands;
using SmartCart.Identity.Application.DTOs;
using SmartCart.Identity.Application.Interfaces;
using SmartCart.Identity.Domain.Entities;

namespace SmartCart.Identity.Application.CommandHandlers
{
    public class RefreshTokenCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponseDto> HandleAsync(RefreshTokenCommand command,CancellationToken cancellationToken = default)
        {
            var existingToken =
                await _userRepository.GetRefreshTokenAsync(
                    command.RefreshToken,
                    cancellationToken);

            if (existingToken == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid refresh token.");
            }

            if (existingToken.IsRevoked)
            {
                throw new UnauthorizedAccessException(
                    "Refresh token has been revoked.");
            }

            if (existingToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException(
                    "Refresh token has expired.");
            }

            var user = existingToken.User;

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "User not found.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException(
                    "User account is inactive.");
            }

            // Revoke previous refresh token
            existingToken.IsRevoked = true;

            // Create new refresh token
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = _jwtTokenService.GenerateRefreshToken(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _userRepository.AddRefreshTokenAsync(newRefreshToken,cancellationToken);

            await _userRepository.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,

                AccessToken = _jwtTokenService.GenerateAccessToken(user),

                RefreshToken = newRefreshToken.Token,

                ExpiresAt = _jwtTokenService.GetAccessTokenExpiry()
            };
        }
    }
}
