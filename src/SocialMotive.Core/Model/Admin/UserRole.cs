namespace SocialMotive.Core.Model.Admin;

public class UserRole
{
    public int UserRoleId { get; set; }
    public int? UserId { get; set; }
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }
}
