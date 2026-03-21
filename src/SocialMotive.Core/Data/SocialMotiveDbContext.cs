using Microsoft.EntityFrameworkCore;
using SocialMotive.Core.Data.Generator;
using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;

namespace SocialMotive.Core.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for SocialMotive database
    /// </summary>
    public class SocialMotiveDbContext : DbContext
    {
        public SocialMotiveDbContext(DbContextOptions<SocialMotiveDbContext> options) : base(options) { }

        // DbSets for all tables
        public DbSet<DbUser> Users { get; set; } = null!;
        public DbSet<DbTracker> Trackers { get; set; } = null!;
        public DbSet<DbEvent> Events { get; set; } = null!;
        public DbSet<DbEventType> EventTypes { get; set; } = null!;
        public DbSet<DbEventTask> EventTasks { get; set; } = null!;
        public DbSet<DbEventSkill> EventSkills { get; set; } = null!;
        public DbSet<DbEventParticipant> EventParticipants { get; set; } = null!;
        public DbSet<DbEventTaskAssignment> EventTaskAssignments { get; set; } = null!;
        public DbSet<DbGroup> Groups { get; set; } = null!;
        public DbSet<DbLabel> Labels { get; set; } = null!;
        public DbSet<DbTrackerLabel> TrackerLabels { get; set; } = null!;
        public DbSet<DbTrackerRole> TrackerRoles { get; set; } = null!;
        public DbSet<DbCity> Cities { get; set; } = null!;
        public DbSet<DbLocation> Locations { get; set; } = null!;
        public DbSet<DbInvite> Invites { get; set; } = null!;
        public DbSet<DbSetting> Settings { get; set; } = null!;
        public DbSet<DbRole> Roles { get; set; } = null!;
        public DbSet<DbUserSocialAccount> UserSocialAccounts { get; set; } = null!;
        public DbSet<DbSocialPlatform> SocialPlatforms { get; set; } = null!;
        public DbSet<DbUserGroup> UserGroups { get; set; } = null!;
        public DbSet<DbUserLabel> UserLabels { get; set; } = null!;
        public DbSet<DbUserRole> UserRoles { get; set; } = null!;
        public DbSet<DbOrganization> Organizations { get; set; } = null!;
        public DbSet<DbOrganizationRole> OrganizationRoles { get; set; } = null!;
        public DbSet<DbOrganizationUser> OrganizationUsers { get; set; } = null!;
        public DbSet<DbVerificationCode> VerificationCodes { get; set; } = null!;

        // Generator tables
        public DbSet<DbTemplate> Templates { get; set; } = null!;
        public DbSet<DbAsset> Assets { get; set; } = null!;

        public DbSet<DbRenderJob> RenderJobs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Primary Keys & Key Configurations =====
            
            modelBuilder.Entity<DbUser>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<DbTracker>()
                .HasKey(t => t.TrackerId);

            modelBuilder.Entity<DbEvent>()
                .HasKey(e => e.EventId);

            modelBuilder.Entity<DbEventType>()
                .HasKey(et => et.EventTypeId);

            modelBuilder.Entity<DbEventTask>()
                .HasKey(et => et.EventTaskId);

            modelBuilder.Entity<DbEventSkill>()
                .HasKey(es => es.EventSkillId);

            modelBuilder.Entity<DbEventParticipant>()
                .HasKey(ep => ep.EventParticipantId);

            modelBuilder.Entity<DbEventTaskAssignment>()
                .HasKey(eta => eta.EventTaskAssignmentId);

            modelBuilder.Entity<DbGroup>()
                .HasKey(g => g.GroupId);

            modelBuilder.Entity<DbLabel>()
                .HasKey(l => l.LabelId);

            modelBuilder.Entity<DbTrackerLabel>()
                .HasKey(tl => tl.TrackerLabelId);

            modelBuilder.Entity<DbTrackerRole>()
                .HasKey(tr => tr.TrackerRoleId);

            modelBuilder.Entity<DbCity>()
                .HasKey(c => c.CityId);

            modelBuilder.Entity<DbLocation>()
                .HasKey(l => l.LocationId);

            modelBuilder.Entity<DbInvite>()
                .HasKey(i => i.InviteId);

            modelBuilder.Entity<DbSetting>()
                .HasKey(s => s.SettingId);

            modelBuilder.Entity<DbRole>()
                .HasKey(r => r.RoleId);

            modelBuilder.Entity<DbUserSocialAccount>()
                .HasKey(ua => ua.UserSocialAccountId);

            modelBuilder.Entity<DbSocialPlatform>()
                .HasKey(sp => sp.SocialPlatformId);

            modelBuilder.Entity<DbUserGroup>()
                .HasKey(ug => ug.UserGroupId);

            modelBuilder.Entity<DbUserLabel>()
                .HasKey(ul => ul.UserLabelId);

            modelBuilder.Entity<DbUserRole>()
                .HasKey(ur => ur.UserRoleId);

            modelBuilder.Entity<DbOrganization>()
                .HasKey(o => o.OrganizationId);

            modelBuilder.Entity<DbOrganizationRole>()
                .HasKey(or => or.OrganizationRoleId);

            modelBuilder.Entity<DbOrganizationUser>()
                .HasKey(ou => ou.OrganizationUserId);

            modelBuilder.Entity<DbVerificationCode>()
                .HasKey(vc => vc.VerificationCodeId);

            // Generator tables
            var templateConfig = modelBuilder.Entity<DbTemplate>();
            templateConfig.ToTable("GeneratorTemplate");
            templateConfig.HasKey(t => t.TemplateId);

            var assetConfig = modelBuilder.Entity<DbAsset>();
            assetConfig.ToTable("GeneratorAsset");
            assetConfig.HasKey(a => a.AssetId);


            var renderJobConfig = modelBuilder.Entity<DbRenderJob>();
            renderJobConfig.ToTable("GeneratorRenderJob");
            renderJobConfig.HasKey(r => r.RenderJobId);

            // ===== Unique Constraints =====

            modelBuilder.Entity<DbUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<DbEventType>()
                .HasIndex(et => et.Name)
                .IsUnique();

            modelBuilder.Entity<DbSetting>()
                .HasIndex(s => s.SettingKey)
                .IsUnique();

            // ===== Foreign Key Relationships =====

            // User → Event (Organizer)
            modelBuilder.Entity<DbEvent>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizerEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // EventType ← Event
            modelBuilder.Entity<DbEvent>()
                .HasOne(e => e.EventType)
                .WithMany(et => et.Events)
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event ← EventTask
            modelBuilder.Entity<DbEventTask>()
                .HasOne(et => et.Event)
                .WithMany(e => e.EventTasks)
                .HasForeignKey(et => et.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // EventSkill ← EventTask
            modelBuilder.Entity<DbEventTask>()
                .HasOne(et => et.EventSkill)
                .WithMany(es => es.EventTasks)
                .HasForeignKey(et => et.EventSkillId)
                .OnDelete(DeleteBehavior.NoAction);

            // Event ← EventParticipant
            modelBuilder.Entity<DbEventParticipant>()
                .HasOne(ep => ep.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // User ← EventParticipant
            modelBuilder.Entity<DbEventParticipant>()
                .HasOne(ep => ep.User)
                .WithMany(u => u.Participations)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // EventTask ← EventTaskAssignment
            modelBuilder.Entity<DbEventTaskAssignment>()
                .HasOne(eta => eta.EventTask)
                .WithMany(et => et.Assignments)
                .HasForeignKey(eta => eta.EventTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // User ← EventTaskAssignment
            modelBuilder.Entity<DbEventTaskAssignment>()
                .HasOne(eta => eta.User)
                .WithMany(u => u.TaskAssignments)
                .HasForeignKey(eta => eta.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Group ← Tracker
            modelBuilder.Entity<DbTracker>()
                .HasOne(t => t.Group)
                .WithMany(g => g.Trackers)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // TrackerRole ← Tracker
            modelBuilder.Entity<DbTracker>()
                .HasOne(t => t.TrackerRole)
                .WithMany(tr => tr.Trackers)
                .HasForeignKey(t => t.TrackerRoleId)
                .OnDelete(DeleteBehavior.SetNull);

            // City ← Tracker
            modelBuilder.Entity<DbTracker>()
                .HasOne(t => t.City)
                .WithMany(c => c.Trackers)
                .HasForeignKey(t => t.CityId)
                .OnDelete(DeleteBehavior.SetNull);

            // Invite ← Tracker
            modelBuilder.Entity<DbTracker>()
                .HasOne(t => t.Invite)
                .WithMany(i => i.Trackers)
                .HasForeignKey(t => t.InviteId)
                .OnDelete(DeleteBehavior.SetNull);

            // VerificationCode ← Tracker (InviteVerificationCode)
            modelBuilder.Entity<DbTracker>()
                .HasOne(t => t.InviteVerificationCode)
                .WithMany()
                .HasForeignKey(t => t.InviteVerificationCodeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Tracker ← Location
            modelBuilder.Entity<DbLocation>()
                .HasOne(l => l.Tracker)
                .WithMany(t => t.Locations)
                .HasForeignKey(l => l.TrackerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tracker ← TrackerLabel
            modelBuilder.Entity<DbTrackerLabel>()
                .HasOne(tl => tl.Tracker)
                .WithMany(t => t.TrackerLabels)
                .HasForeignKey(tl => tl.TrackerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Label ← TrackerLabel
            modelBuilder.Entity<DbTrackerLabel>()
                .HasOne(tl => tl.Label)
                .WithMany(l => l.TrackerLabels)
                .HasForeignKey(tl => tl.LabelId)
                .OnDelete(DeleteBehavior.Cascade);

            // User ← UserSocialAccount
            modelBuilder.Entity<DbUserSocialAccount>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.SocialAccounts)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // SocialPlatform ← UserSocialAccount
            modelBuilder.Entity<DbUserSocialAccount>()
                .HasOne(ua => ua.SocialPlatform)
                .WithMany(sp => sp.UserSocialAccounts)
                .HasForeignKey(ua => ua.SocialPlatformId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ← UserGroup
            modelBuilder.Entity<DbUserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Group ← UserGroup
            modelBuilder.Entity<DbUserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(ug => ug.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ← UserLabel
            modelBuilder.Entity<DbUserLabel>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.UserLabels)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Label ← UserLabel
            modelBuilder.Entity<DbUserLabel>()
                .HasOne(ul => ul.Label)
                .WithMany(l => l.UserLabels)
                .HasForeignKey(ul => ul.LabelId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ← UserRole
            modelBuilder.Entity<DbUserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Role ← UserRole
            modelBuilder.Entity<DbUserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ← Organization (OwnedBy)
            modelBuilder.Entity<DbOrganization>()
                .HasOne(o => o.Owner)
                .WithMany()
                .HasForeignKey(o => o.OwnedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // User ← Organization (CreatedBy)
            modelBuilder.Entity<DbOrganization>()
                .HasOne(o => o.Creator)
                .WithMany()
                .HasForeignKey(o => o.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // User ← Organization (ModifiedBy)
            modelBuilder.Entity<DbOrganization>()
                .HasOne(o => o.Modifier)
                .WithMany()
                .HasForeignKey(o => o.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // User ← OrganizationUser
            modelBuilder.Entity<DbOrganizationUser>()
                .HasOne(ou => ou.User)
                .WithMany()
                .HasForeignKey(ou => ou.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrganizationRole ← OrganizationUser
            modelBuilder.Entity<DbOrganizationUser>()
                .HasOne(ou => ou.OrganizationRole)
                .WithMany(or => or.OrganizationUsers)
                .HasForeignKey(ou => ou.OrganizationRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ← OrganizationUser (AssignedBy)
            modelBuilder.Entity<DbOrganizationUser>()
                .HasOne(ou => ou.AssignedByUser)
                .WithMany()
                .HasForeignKey(ou => ou.AssignedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // VerificationCode ← User
            modelBuilder.Entity<DbVerificationCode>()
                .HasOne(vc => vc.User)
                .WithMany()
                .HasForeignKey(vc => vc.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Template ← User
            modelBuilder.Entity<DbTemplate>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Asset ← User
            modelBuilder.Entity<DbAsset>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            // RenderJob ← Template
            modelBuilder.Entity<DbRenderJob>()
                .HasOne(r => r.Template)
                .WithMany(t => t.RenderJobs)
                .HasForeignKey(r => r.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // RenderJob ← User
            modelBuilder.Entity<DbRenderJob>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ===== Table Names & Schema =====
            
            modelBuilder.Entity<DbUser>().ToTable("Users");
            modelBuilder.Entity<DbTracker>().ToTable("Trackers");
            modelBuilder.Entity<DbEvent>().ToTable("Events");
            modelBuilder.Entity<DbEventType>().ToTable("EventTypes");
            modelBuilder.Entity<DbEventTask>().ToTable("EventTasks");
            modelBuilder.Entity<DbEventParticipant>().ToTable("EventParticipants");
            modelBuilder.Entity<DbEventSkill>().ToTable("EventSkills");
            modelBuilder.Entity<DbEventTaskAssignment>().ToTable("EventTaskAssignments");
            modelBuilder.Entity<DbGroup>().ToTable("Groups");
            modelBuilder.Entity<DbLabel>().ToTable("Labels");
            modelBuilder.Entity<DbTrackerLabel>().ToTable("TrackerLabels");
            modelBuilder.Entity<DbTrackerRole>().ToTable("TrackerRoles");
            modelBuilder.Entity<DbCity>().ToTable("Cities");
            modelBuilder.Entity<DbLocation>().ToTable("Locations");
            modelBuilder.Entity<DbInvite>().ToTable("Invites");
            modelBuilder.Entity<DbSetting>().ToTable("Settings");
            modelBuilder.Entity<DbRole>().ToTable("Roles");
            modelBuilder.Entity<DbUserSocialAccount>().ToTable("UserSocialAccounts");
            modelBuilder.Entity<DbSocialPlatform>().ToTable("SocialPlatforms");
            modelBuilder.Entity<DbUserGroup>().ToTable("UserGroups");
            modelBuilder.Entity<DbUserLabel>().ToTable("UserLabels");
            modelBuilder.Entity<DbUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<DbOrganization>().ToTable("Organizations");
            modelBuilder.Entity<DbOrganizationRole>().ToTable("OrganizationRoles");
            modelBuilder.Entity<DbOrganizationUser>().ToTable("OrganizationUsers");
            modelBuilder.Entity<DbVerificationCode>().ToTable("VerificationCodes");

            // ===== Column / Property Configurations =====

            // User email is unique and required
            modelBuilder.Entity<DbUser>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<DbUser>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<DbUser>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<DbUser>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            // Indexes for common queries
            modelBuilder.Entity<DbEvent>()
                .HasIndex(e => e.OrganizerId);

            modelBuilder.Entity<DbEvent>()
                .HasIndex(e => e.StartDate);

            modelBuilder.Entity<DbEvent>()
                .HasIndex(e => e.Status);

            modelBuilder.Entity<DbEventParticipant>()
                .HasIndex(ep => ep.EventId);

            modelBuilder.Entity<DbEventParticipant>()
                .HasIndex(ep => ep.UserId);

            modelBuilder.Entity<DbEventParticipant>()
                .Property(ep => ep.HoursWorked)
                .HasPrecision(8, 2);

            modelBuilder.Entity<DbEvent>()
                .Property(e => e.HoursEstimate)
                .HasPrecision(8, 2);

            modelBuilder.Entity<DbEventTask>()
                .Property(et => et.HoursEstimate)
                .HasPrecision(8, 2);

            modelBuilder.Entity<DbEventTaskAssignment>()
                .HasIndex(eta => eta.EventTaskId);

            modelBuilder.Entity<DbEventTaskAssignment>()
                .HasIndex(eta => eta.UserId);

            modelBuilder.Entity<DbTracker>()
                .HasIndex(t => t.Email);

            modelBuilder.Entity<DbTracker>()
                .HasIndex(t => t.GroupId);

            modelBuilder.Entity<DbLocation>()
                .HasIndex(l => l.TrackerId);

            modelBuilder.Entity<DbLocation>()
                .HasIndex(l => l.Timestamp);

            modelBuilder.Entity<DbTrackerLabel>()
                .HasIndex(tl => new { tl.TrackerId, tl.LabelId });

            // Generator table indexes
            modelBuilder.Entity<DbTemplate>()
                .HasIndex(t => t.UserId);

            modelBuilder.Entity<DbTemplate>()
                .HasIndex(t => t.IsPublished);

            modelBuilder.Entity<DbAsset>()
                .HasIndex(a => a.UserId);

            modelBuilder.Entity<DbAsset>()
                .HasIndex(a => a.IsPublic);


            modelBuilder.Entity<DbRenderJob>()
                .HasIndex(r => r.TemplateId);

            modelBuilder.Entity<DbRenderJob>()
                .HasIndex(r => r.UserId);

            modelBuilder.Entity<DbRenderJob>()
                .HasIndex(r => r.Status);

            modelBuilder.Entity<DbUserSocialAccount>()
                .HasIndex(ua => ua.UserId);

            modelBuilder.Entity<DbUserSocialAccount>()
                .HasIndex(ua => ua.SocialPlatformId);

            // VerificationCodes indexes
            modelBuilder.Entity<DbVerificationCode>()
                .Property(vc => vc.Code)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<DbVerificationCode>()
                .Property(vc => vc.CodeType)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<DbVerificationCode>()
                .Property(vc => vc.Target)
                .HasMaxLength(255);

            modelBuilder.Entity<DbVerificationCode>()
                .HasIndex(vc => vc.Code);

            modelBuilder.Entity<DbVerificationCode>()
                .HasIndex(vc => vc.UserId);

            modelBuilder.Entity<DbVerificationCode>()
                .HasIndex(vc => vc.CodeType);

            modelBuilder.Entity<DbVerificationCode>()
                .HasIndex(vc => vc.ExpiresAt);
        }
    }
}
