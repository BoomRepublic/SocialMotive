namespace SocialMotive.Core.Model.Admin;

public class UserGroup
{
    public int UserGroupId { get; set; }
    public int? UserId { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
}
