using System.ComponentModel.DataAnnotations;

namespace SocialMotive.Core.Model.Volunteer
{
    /// <summary>
    /// DTO for volunteer to update notes on a task assignment
    /// </summary>
    public class TaskAssignmentUpdate
    {
        [StringLength(2000)]
        public string? Notes { get; set; }
    }
}
