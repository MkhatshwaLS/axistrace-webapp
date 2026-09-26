using AxisTrace.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskStatus = AxisTrace.Api.Models.TaskStatus;

namespace AxisTrace.Api.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();

            if (context.Users.Any())
            {
                return;
            }

            var passwordHasher = new PasswordHasher<User>();

            var users = new List<User>
            {
                new User
                {
                    Username = "alexmorgan",
                    Email = "alex.morgan@axistrace.io",
                    FirstName = "Alex",
                    LastName = "Morgan",
                    Role = UserRole.ProjectManager,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-45)
                },
                new User
                {
                    Username = "maya.chen",
                    Email = "maya.chen@axistrace.io",
                    FirstName = "Maya",
                    LastName = "Chen",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                },
                new User
                {
                    Username = "samirpatel",
                    Email = "samir.patel@axistrace.io",
                    FirstName = "Samir",
                    LastName = "Patel",
                    Role = UserRole.Contractor,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new User
                {
                    Username = "nataliegray",
                    Email = "natalie.gray@axistrace.io",
                    FirstName = "Natalie",
                    LastName = "Gray",
                    Role = UserRole.Client,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                }
            };

            foreach (var user in users)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            var owner = users[0];
            var projectOne = new Project
            {
                Name = "Customer Portal Redesign",
                Description = "Upgrading the client-facing portal experience with easier onboarding and clearer project tracking.",
                OwnerId = owner.Id,
                StartDate = DateTime.UtcNow.AddDays(-40),
                EndDate = DateTime.UtcNow.AddDays(25),
                EstimatedCompletionDate = DateTime.UtcNow.AddDays(25),
                Status = ProjectStatus.InProgress,
                Location = "Remote",
                Budget = 42000m,
                CreatedAt = DateTime.UtcNow.AddDays(-40)
            };

            var projectTwo = new Project
            {
                Name = "AI Assistant Beta",
                Description = "Pilot launch of an internal AI assistant for project status, notes, and task summaries.",
                OwnerId = users[1].Id,
                StartDate = DateTime.UtcNow.AddDays(-22),
                EndDate = DateTime.UtcNow.AddDays(18),
                EstimatedCompletionDate = DateTime.UtcNow.AddDays(18),
                Status = ProjectStatus.OnHold,
                Location = "Hybrid",
                Budget = 27500m,
                CreatedAt = DateTime.UtcNow.AddDays(-22)
            };

            var projectThree = new Project
            {
                Name = "Operations Dashboard Refresh",
                Description = "Modernizing the executive dashboard for KPI visibility and weekly operational reporting.",
                OwnerId = users[2].Id,
                StartDate = DateTime.UtcNow.AddDays(-60),
                EndDate = DateTime.UtcNow.AddDays(-5),
                EstimatedCompletionDate = DateTime.UtcNow.AddDays(-5),
                Status = ProjectStatus.Completed,
                Location = "HQ",
                Budget = 31000m,
                CreatedAt = DateTime.UtcNow.AddDays(-60)
            };

            context.Projects.AddRange(projectOne, projectTwo, projectThree);
            context.SaveChanges();

            var milestones = new List<Milestone>
            {
                new Milestone
                {
                    Name = "Discovery and UX Review",
                    Description = "Client workshops and UX review for portal improvements.",
                    ProjectId = projectOne.Id,
                    DueDate = DateTime.UtcNow.AddDays(-10),
                    CompletedAt = DateTime.UtcNow.AddDays(-8),
                    Status = MilestoneStatus.Completed,
                    ProgressPercentage = 100,
                    Order = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-35)
                },
                new Milestone
                {
                    Name = "Portal Build Sprint",
                    Description = "Core implementation of redesign components and user flows.",
                    ProjectId = projectOne.Id,
                    DueDate = DateTime.UtcNow.AddDays(12),
                    Status = MilestoneStatus.InProgress,
                    ProgressPercentage = 68,
                    Order = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-18)
                },
                new Milestone
                {
                    Name = "Pilot Feedback Review",
                    Description = "Review beta feedback and prepare final release checklist.",
                    ProjectId = projectTwo.Id,
                    DueDate = DateTime.UtcNow.AddDays(8),
                    Status = MilestoneStatus.InProgress,
                    ProgressPercentage = 42,
                    Order = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new Milestone
                {
                    Name = "Launch Readiness",
                    Description = "Completion of reporting and KPI widgets for executive dashboard.",
                    ProjectId = projectThree.Id,
                    DueDate = DateTime.UtcNow.AddDays(-3),
                    CompletedAt = DateTime.UtcNow.AddDays(-2),
                    Status = MilestoneStatus.Completed,
                    ProgressPercentage = 100,
                    Order = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-40)
                }
            };

            context.Milestones.AddRange(milestones);
            context.SaveChanges();

            var tasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Name = "Review onboarding flow",
                    Description = "Validate the onboarding design against the customer journey map.",
                    MilestoneId = milestones[1].Id,
                    AssignedUserId = users[2].Id,
                    DueDate = DateTime.UtcNow.AddDays(4),
                    Status = TaskStatus.InProgress,
                    Priority = TaskPriority.High,
                    EstimatedHours = 16,
                    ActualHours = 8,
                    CreatedAt = DateTime.UtcNow.AddDays(-12)
                },
                new ProjectTask
                {
                    Name = "Update project status widgets",
                    Description = "Apply final KPI layouts and filter logic for dashboard cards.",
                    MilestoneId = milestones[3].Id,
                    AssignedUserId = users[0].Id,
                    DueDate = DateTime.UtcNow.AddDays(-1),
                    CompletedAt = DateTime.UtcNow.AddDays(-1),
                    Status = TaskStatus.Completed,
                    Priority = TaskPriority.Medium,
                    EstimatedHours = 12,
                    ActualHours = 10,
                    CreatedAt = DateTime.UtcNow.AddDays(-28)
                },
                new ProjectTask
                {
                    Name = "Run beta feedback session",
                    Description = "Document user pain points from the latest pilot feedback session.",
                    MilestoneId = milestones[2].Id,
                    AssignedUserId = users[1].Id,
                    DueDate = DateTime.UtcNow.AddDays(6),
                    Status = TaskStatus.InReview,
                    Priority = TaskPriority.High,
                    EstimatedHours = 10,
                    ActualHours = 6,
                    CreatedAt = DateTime.UtcNow.AddDays(-9)
                },
                new ProjectTask
                {
                    Name = "Finalize portal accessibility pass",
                    Description = "Review keyboard and contrast accessibility across the new layouts.",
                    MilestoneId = milestones[1].Id,
                    AssignedUserId = users[3].Id,
                    DueDate = DateTime.UtcNow.AddDays(9),
                    Status = TaskStatus.NotStarted,
                    Priority = TaskPriority.Medium,
                    EstimatedHours = 8,
                    ActualHours = 0,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new ProjectTask
                {
                    Name = "Prepare launch summary deck",
                    Description = "Summarize dashboard performance metrics and outcomes for stakeholders.",
                    MilestoneId = milestones[3].Id,
                    AssignedUserId = users[1].Id,
                    DueDate = DateTime.UtcNow.AddDays(-4),
                    CompletedAt = DateTime.UtcNow.AddDays(-3),
                    Status = TaskStatus.Completed,
                    Priority = TaskPriority.High,
                    EstimatedHours = 14,
                    ActualHours = 11,
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();

            context.Comments.AddRange(
                new Comment
                {
                    Content = "The new onboarding flow feels much clearer. We should keep the checklist style for the final release.",
                    UserId = users[3].Id,
                    ProjectId = projectOne.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                },
                new Comment
                {
                    Content = "The beta feedback confirms our users need more automation in status summaries.",
                    UserId = users[1].Id,
                    ProjectId = projectTwo.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                new Comment
                {
                    Content = "Dashboard launch is complete and stakeholders approved the KPI layout.",
                    UserId = users[0].Id,
                    ProjectId = projectThree.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            );

            context.ProgressUpdates.AddRange(
                new ProgressUpdate
                {
                    Description = "Portal redesign is 68% complete with the onboarding flow in review.",
                    UserId = users[0].Id,
                    ProjectId = projectOne.Id,
                    ProgressPercentage = 68,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new ProgressUpdate
                {
                    Description = "The AI assistant pilot is progressing with feedback collection underway.",
                    UserId = users[1].Id,
                    ProjectId = projectTwo.Id,
                    ProgressPercentage = 42,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new ProgressUpdate
                {
                    Description = "Executive dashboard refresh passed final stakeholder approval.",
                    UserId = users[2].Id,
                    ProjectId = projectThree.Id,
                    ProgressPercentage = 100,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            );

            context.SaveChanges();
        }
    }
}
