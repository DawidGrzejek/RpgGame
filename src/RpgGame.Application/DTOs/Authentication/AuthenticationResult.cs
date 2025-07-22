using RpgGame.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Application.DTOs.Authentication
{
    public class AuthenticationResult : OperationResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = new();

        // Constructor for success
        public AuthenticationResult(bool succeeded, object? data = null)
            : base(succeeded, data)
        { }

        // Constructor for single error
        public AuthenticationResult(OperationError error)
            : base(error)
        { }

        // Constructor for multiple errors
        public AuthenticationResult(IEnumerable<OperationError> errors)
            : base(errors)
        { }

        // Expose error messaged for API consumers
        public List<string> Errors => ErrorMessages;
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}