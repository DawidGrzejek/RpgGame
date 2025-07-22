using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RpgGame.Application.DTOs.Authentication;

namespace RpgGame.Application.Commands.Users
{
    /// <summary>
    /// Represents a command to register a new user with the specified credentials.
    /// </summary>
    /// <remarks>This command is used to initiate the user registration process. It encapsulates the necessary
    /// information, such as the username, email, and password, required to create a new user account.</remarks>
    /// <param name="Username">The desired username for the new user. Cannot be null or empty.</param>
    /// <param name="Email">The email address of the new user. Must be a valid email format and cannot be null or empty.</param>
    /// <param name="Password">The password for the new user. Must meet the application's password complexity requirements and cannot be null
    /// or empty.</param>
    public record RegisterUserCommand(string Username, string Email, string Password) : IRequest<AuthenticationResult>;

}