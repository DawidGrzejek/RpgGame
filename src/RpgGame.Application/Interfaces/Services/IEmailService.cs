using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string email, string username, CancellationToken cancellationToken = default);
        Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken = default);
    }
}