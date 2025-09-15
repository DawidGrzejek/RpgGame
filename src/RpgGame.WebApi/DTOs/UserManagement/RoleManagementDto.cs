using System.ComponentModel.DataAnnotations;

namespace RpgGame.WebApi.DTOs.UserManagement
{

    public class CreateRoleRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateRoleRequest
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }

    public class AssignRoleRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string RoleName { get; set; } = string.Empty;
    }

    public class RemoveRoleRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}