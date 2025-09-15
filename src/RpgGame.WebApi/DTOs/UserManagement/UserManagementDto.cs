using System.ComponentModel.DataAnnotations;

namespace RpgGame.WebApi.DTOs.UserManagement
{

    public class CreateUserRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 12)]
        public string Password { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new();
        public bool EmailConfirmed { get; set; } = false;
        public bool LockoutEnabled { get; set; } = true;
    }

    public class UpdateUserRequest
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool IsActive { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 12)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UserLockoutRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTimeOffset? LockoutEnd { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}