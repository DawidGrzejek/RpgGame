using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RpgGame.Application.Commands.Users;
using RpgGame.Application.DTOs.Authentication;
using RpgGame.Application.Interfaces.Authentication;
using RpgGame.Application.Interfaces.Repositories;

namespace RpgGame.Application.Handlers.Users
{
    public class LoginHandler : IRequestHandler<LoginCommand, AuthenticationResult>
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserRepository _userRepository;

        public LoginHandler(IAuthenticationService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request.Email, request.Password);
            
            if (result.Succeeded)
            {
                // Update domain user's last login
                var user = await _userRepository.GetByEmailAsync(request.Email);
                user?.RecordLogin();
                await _userRepository.SaveChangesAsync();
            }

            return result;
        }
    }
}