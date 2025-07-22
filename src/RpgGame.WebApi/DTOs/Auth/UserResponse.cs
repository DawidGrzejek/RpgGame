namespace RpgGame.WebApi.DTOs.Auth
{
    /// <summary>
    /// Response model for user information.
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// The unique identifier of the user.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The username of the user.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The roles assigned to the user.
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// The date when the user account was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The date of the user's last login.
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Indicates whether the user account is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
