using Microsoft.EntityFrameworkCore;
using SocialMotive.WebApp.Models;

namespace SocialMotive.WebApp.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for SocialMotive database
    /// </summary>
    public class SocialMotiveDbContext : DbContext
    {
        public SocialMotiveDbContext(DbContextOptions<SocialMotiveDbContext> options) : base(options) { }

        // DbSets for all tables
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Tracker> Trackers { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventType> EventTypes { get; set; } = null!;
        public DbSet<EventTask> EventTasks { get; set; } = null!;
        public DbSet<EventParticipant> EventParticipants { get; set; } = null!;
        public DbSet<EventTaskAssignment> EventTaskAssignments { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<Label> Labels { get; set; } = null!;
        public DbSet<TrackerLabel> TrackerLabels { get; set; } = null!;
        public DbSet<TrackerRole> TrackerRoles { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Invite> Invites { get; set; } = null!;
        public DbSet<Setting> Settings { get; set; } = null!;
        public DbSet<UserAccount> UserAccounts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Primary Keys & Key Configurations =====
            
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Tracker>()
                .HasKey(t => t.TrackerId);

            modelBuilder.Entity<Event>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<EventType>()
                .HasKey(et => et.Id);

            modelBuilder.Entity<EventTask>()
                .HasKey(et => et.Id);

            modelBuilder.Entity<EventParticipant>()
                .HasKey(ep => ep.Id);

            modelBuilder.Entity<EventTaskAssignment>()
                .HasKey(eta => eta.Id);

            modelBuilder.Entity<Group>()
                .HasKey(g => g.GroupId);

            modelBuilder.Entity<Label>()
                .HasKey(l => l.LabelId);

            modelBuilder.Entity<TrackerLabel>()
                .HasKey(tl => tl.TrackerLabelId);

            modelBuilder.Entity<TrackerRole>()
                .HasKey(tr => tr.TrackerRoleId);

            modelBuilder.Entity<City>()
                .HasKey(c => c.CityId);

            modelBuilder.Entity<Location>()
                .HasKey(l => l.LocationId);

            modelBuilder.Entity<Invite>()
                .HasKey(i => i.InviteId);

            modelBuilder.Entity<Setting>()
                .HasKey(s => s.SettingId);

            modelBuilder.Entity<UserAccount>()
                .HasKey(ua => ua.Id);

            // ===== Unique Constraints =====

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<EventType>()
                .HasIndex(et => et.Name)
                .IsUnique();

            modelBuilder.Entity<Setting>()
                .HasIndex(s => s.SettingKey)
                .IsUnique();

            modelBuilder.Entity<Tracker>()
                .HasIndex(t => t.QrGuid)
                .IsUnique();

            // ===== Foreign Key Relationships =====

            // User → Event (Organizer)
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizerEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // EventType ← Event
            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany(et => et.Events)
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event ← EventTask
            modelBuilder.Entity<EventTask>()
                .HasOne(et => et.Event)
                .WithMany(e => e.EventTasks)
                .HasForeignKey(et => et.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Event ← EventParticipant
            modelBuilder.Entity<EventParticipant>()
                .HasOne(ep => ep.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // User ← EventParticipant
            modelBuilder.Entity<EventParticipant>()
                .HasOne(ep => ep.User)
                .WithMany(u => u.Participations)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // EventTask ← EventTaskAssignment
            modelBuilder.Entity<EventTaskAssignment>()
                .HasOne(eta => eta.EventTask)
                .WithMany(et => et.Assignments)
                .HasForeignKey(eta => eta.EventTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // User ← EventTaskAssignment
            modelBuilder.Entity<EventTaskAssignment>()
                .HasOne(eta => eta.User)
                .WithMany(u => u.TaskAssignments)
                .HasForeignKey(eta => eta.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Group ← Tracker
            modelBuilder.Entity<Tracker>()
                .HasOne(t => t.Group)
                .WithMany(g => g.Trackers)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // TrackerRole ← Tracker
            modelBuilder.Entity<Tracker>()
                .HasOne(t => t.TrackerRole)
                .WithMany(tr => tr.Trackers)
                .HasForeignKey(t => t.TrackerRoleId)
                .OnDelete(DeleteBehavior.SetNull);

            // City ← Tracker
            modelBuilder.Entity<Tracker>()
                .HasOne(t => t.City)
                .WithMany(c => c.Trackers)
                .HasForeignKey(t => t.CityId)
                .OnDelete(DeleteBehavior.SetNull);

            // Invite ← Tracker
            modelBuilder.Entity<Tracker>()
                .HasOne(t => t.Invite)
                .WithMany(i => i.Trackers)
                .HasForeignKey(t => t.InviteId)
                .OnDelete(DeleteBehavior.SetNull);

            // Tracker ← Location
            modelBuilder.Entity<Location>()
                .HasOne(l => l.Tracker)
                .WithMany(t => t.Locations)
                .HasForeignKey(l => l.TrackerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tracker ← TrackerLabel
            modelBuilder.Entity<TrackerLabel>()
                .HasOne(tl => tl.Tracker)
                .WithMany(t => t.TrackerLabels)
                .HasForeignKey(tl => tl.TrackerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Label ← TrackerLabel
            modelBuilder.Entity<TrackerLabel>()
                .HasOne(tl => tl.Label)
                .WithMany(l => l.TrackerLabels)
                .HasForeignKey(tl => tl.LabelId)
                .OnDelete(DeleteBehavior.Cascade);

            // User ← UserAccount
            modelBuilder.Entity<UserAccount>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Table Names & Schema =====
            
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Tracker>().ToTable("Trackers");
            modelBuilder.Entity<Event>().ToTable("Events");
            modelBuilder.Entity<EventType>().ToTable("EventTypes");
            modelBuilder.Entity<EventTask>().ToTable("EventTasks");
            modelBuilder.Entity<EventParticipant>().ToTable("EventParticipants");
            modelBuilder.Entity<EventTaskAssignment>().ToTable("EventTaskAssignments");
            modelBuilder.Entity<Group>().ToTable("Groups");
            modelBuilder.Entity<Label>().ToTable("Labels");
            modelBuilder.Entity<TrackerLabel>().ToTable("TrackerLabels");
            modelBuilder.Entity<TrackerRole>().ToTable("TrackerRoles");
            modelBuilder.Entity<City>().ToTable("Cities");
            modelBuilder.Entity<Location>().ToTable("Locations");
            modelBuilder.Entity<Invite>().ToTable("Invites");
            modelBuilder.Entity<Setting>().ToTable("Settings");
            modelBuilder.Entity<UserAccount>().ToTable("UserAccounts");

            // ===== Column / Property Configurations =====

            // User email is unique and required
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            // Indexes for common queries
            modelBuilder.Entity<Event>()
                .HasIndex(e => e.OrganizerId);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.StartDate);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Status);

            modelBuilder.Entity<EventParticipant>()
                .HasIndex(ep => ep.EventId);

            modelBuilder.Entity<EventParticipant>()
                .HasIndex(ep => ep.UserId);

            modelBuilder.Entity<EventTaskAssignment>()
                .HasIndex(eta => eta.EventTaskId);

            modelBuilder.Entity<EventTaskAssignment>()
                .HasIndex(eta => eta.UserId);

            modelBuilder.Entity<Tracker>()
                .HasIndex(t => t.Email);

            modelBuilder.Entity<Tracker>()
                .HasIndex(t => t.GroupId);

            modelBuilder.Entity<Location>()
                .HasIndex(l => l.TrackerId);

            modelBuilder.Entity<Location>()
                .HasIndex(l => l.Timestamp);

            modelBuilder.Entity<TrackerLabel>()
                .HasIndex(tl => new { tl.TrackerId, tl.LabelId });
        }
    }
}
