using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RpgGame.Application.DTOs.Authentication;

namespace RpgGame.Application.Queries.Users
{
    public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;
}