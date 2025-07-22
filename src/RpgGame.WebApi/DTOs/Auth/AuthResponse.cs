namespace RpgGame.WebApi.DTOs.Auth
{
    /// <summary>
    /// Response model for authentication operations.
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Indicates whether the authentication operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The JWT access token for authenticated requests.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// The refresh token for obtaining new access tokens.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// The expiration time of the access token.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// The authenticated user's information.
        /// </summary>
        public UserResponse? User { get; set; }

        /// <summary>
        /// List of error messages if the operation failed.
        /// </summary>
        public List<string> Errors { get; set; } = new();
    }
}
