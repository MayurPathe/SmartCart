using MediatR;
using SmartCart.Identity.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Application.Features.Auth.Commands.Register;

public record RegisterUserCommand(string FullName, string Email, string Password, string PhoneNumber) : IRequest<RegisterResponse>;

