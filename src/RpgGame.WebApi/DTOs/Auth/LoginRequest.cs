using System.ComponentModel.DataAnnotations;
using RpgGame.WebApi.Validators;

namespace RpgGame.WebApi.DTOs.Auth
{
    /// <summary>
    /// Request model for user login.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// The email address of the user.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EnhancedEmail(ErrorMessage = "Please provide a valid email address with proper domain (e.g., user@example.com)")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The password of the user.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
