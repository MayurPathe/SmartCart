using MediatR;
using SmartCart.Identity.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Application.Features.Auth.Commands.Login;

public record LoginUserCommand(string Email,string Password,string? IpAddress,string? UserAgent)
  : IRequest<AuthResponse>;
