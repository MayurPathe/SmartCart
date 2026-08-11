using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCart.Identity.Application.Commands;
using SmartCart.Identity.Application.Interfaces;

namespace SmartCart.Identity.Application.CommandHandlers
{
    public class LogoutCommandHandler
    {
        private readonly IUserRepository _userRepository;

        public LogoutCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(LogoutCommand command,CancellationToken cancellationToken = default)
        {
            var refreshToken =
                await _userRepository.GetRefreshTokenAsync(
                    command.RefreshToken,
                    cancellationToken);

            if (refreshToken == null)
            {
                return;
            }

            if (!refreshToken.IsRevoked)
            {
                refreshToken.IsRevoked = true;

                await _userRepository.SaveChangesAsync(
                    cancellationToken);
            }
        }
    }
}