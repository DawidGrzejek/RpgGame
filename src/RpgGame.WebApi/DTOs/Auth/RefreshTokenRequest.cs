using System.ComponentModel.DataAnnotations;

namespace RpgGame.WebApi.DTOs.Auth
{
    /// <summary>
    /// Request model for token refresh.
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// The refresh token to use for generating a new access token.
        /// </summary>
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
