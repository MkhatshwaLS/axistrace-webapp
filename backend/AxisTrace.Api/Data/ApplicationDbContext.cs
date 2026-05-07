using Microsoft.EntityFrameworkCore;
using AxisTrace.Api.Models;

namespace AxisTrace.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<ProjectTask> Tasks { get; set; }
        public DbSet<ProgressUpdate> ProgressUpdates { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints

            // User relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.OwnedProjects)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.AssignedTasks)
                .WithOne(t => t.AssignedUser)
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ProgressUpdates)
                .WithOne(pu => pu.User)
                .HasForeignKey(pu => pu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Project relationships
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Milestones)
                .WithOne(m => m.Project)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Project)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Milestone relationships
            modelBuilder.Entity<Milestone>()
                .HasMany(m => m.Tasks)
                .WithOne(t => t.Milestone)
                .HasForeignKey(t => t.MilestoneId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Milestone>()
                .HasMany(m => m.Comments)
                .WithOne(c => c.Milestone)
                .HasForeignKey(c => c.MilestoneId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Milestone>()
                .HasMany(m => m.ProgressUpdates)
                .WithOne(pu => pu.Milestone)
                .HasForeignKey(pu => pu.MilestoneId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task relationships
            modelBuilder.Entity<ProjectTask>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.Task)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectTask>()
                .HasMany(t => t.ProgressUpdates)
                .WithOne(pu => pu.Task)
                .HasForeignKey(pu => pu.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment relationships (for threaded comments)
            modelBuilder.Entity<Comment>()
                .HasMany(c => c.Replies)
                .WithOne(c => c.ParentComment)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProgressUpdate relationships (ensure only one entity is referenced)
            modelBuilder.Entity<ProgressUpdate>()
                .HasCheckConstraint("CK_ProgressUpdate_SingleReference",
                    "(ProjectId IS NOT NULL AND MilestoneId IS NULL AND TaskId IS NULL) OR " +
                    "(ProjectId IS NULL AND MilestoneId IS NOT NULL AND TaskId IS NULL) OR " +
                    "(ProjectId IS NULL AND MilestoneId IS NULL AND TaskId IS NOT NULL)");

            // Indexes for performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.OwnerId);

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Status);

            modelBuilder.Entity<Milestone>()
                .HasIndex(m => m.ProjectId);

            modelBuilder.Entity<Milestone>()
                .HasIndex(m => m.Status);

            modelBuilder.Entity<Task>()
                .HasIndex(t => t.MilestoneId);

            modelBuilder.Entity<Task>()
                .HasIndex(t => t.AssignedUserId);

            modelBuilder.Entity<Task>()
                .HasIndex(t => t.Status);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.UserId);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.ProjectId);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.MilestoneId);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.TaskId);

            modelBuilder.Entity<ProgressUpdate>()
                .HasIndex(pu => pu.UserId);

            modelBuilder.Entity<ProgressUpdate>()
                .HasIndex(pu => pu.ProjectId);

            modelBuilder.Entity<ProgressUpdate>()
                .HasIndex(pu => pu.MilestoneId);

            modelBuilder.Entity<ProgressUpdate>()
                .HasIndex(pu => pu.TaskId);
        }
    }
}