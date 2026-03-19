namespace SocialMotive.Core.Model.Admin;

public class UserLabel
{
    public int UserLabelId { get; set; }
    public int? UserId { get; set; }
    public int? LabelId { get; set; }
    public string? LabelName { get; set; }
}
