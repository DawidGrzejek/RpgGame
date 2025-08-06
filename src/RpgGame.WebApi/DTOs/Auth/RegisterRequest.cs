using System.ComponentModel.DataAnnotations;
using RpgGame.WebApi.Validators;

namespace RpgGame.WebApi.DTOs.Auth
{
    /// <summary>
    /// Request model for user registration.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// The desired username for the new account.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The email address for the new account.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EnhancedEmail(ErrorMessage = "Please provide a valid email address with proper domain (e.g., user@example.com)")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The password for the new account.
        /// (?=.*[a-z]) — contains lowercase letters: a, b, c, ...
        /// (?=.*[A-Z]) — contains uppercase letters: P, Q, R, ...
        /// (?=.*\d) — contains digits: 0, 1, 2, 3, ...
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation to ensure accuracy.
        /// </summary>
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
