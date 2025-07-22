using MediatR;
using RpgGame.Application.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Users
{
    public record LoginCommand(
        string Email,
        string Password
    ) : IRequest<AuthenticationResult>;
}
