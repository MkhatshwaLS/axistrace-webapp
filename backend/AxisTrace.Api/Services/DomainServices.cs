using AxisTrace.Api.Data;
using AxisTrace.Api.DTOs;
using AxisTrace.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskStatus = AxisTrace.Api.Models.TaskStatus;

namespace AxisTrace.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }

    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> CreateUserAsync(CreateUserDto dto);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
    }

    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetProjectsAsync();
        Task<ProjectDto?> GetProjectByIdAsync(int id);
        Task<ProjectDto?> CreateProjectAsync(CreateProjectDto dto);
        Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto dto);
        Task<bool> DeleteProjectAsync(int id);
    }

    public interface IMilestoneService
    {
        Task<IEnumerable<MilestoneDto>> GetMilestonesAsync();
        Task<MilestoneDto?> GetMilestoneByIdAsync(int id);
        Task<MilestoneDto?> CreateMilestoneAsync(CreateMilestoneDto dto);
        Task<MilestoneDto?> UpdateMilestoneAsync(int id, UpdateMilestoneDto dto);
        Task<bool> DeleteMilestoneAsync(int id);
    }

    public interface IProjectTaskService
    {
        Task<IEnumerable<ProjectTaskDto>> GetTasksAsync();
        Task<ProjectTaskDto?> GetTaskByIdAsync(int id);
        Task<ProjectTaskDto?> CreateTaskAsync(CreateProjectTaskDto dto);
        Task<ProjectTaskDto?> UpdateTaskAsync(int id, UpdateProjectTaskDto dto);
        Task<bool> DeleteTaskAsync(int id);
    }

    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetCommentsAsync();
        Task<CommentDto?> GetCommentByIdAsync(int id);
        Task<CommentDto?> CreateCommentAsync(CreateCommentDto dto);
        Task<CommentDto?> UpdateCommentAsync(int id, UpdateCommentDto dto);
        Task<bool> DeleteCommentAsync(int id);
    }

    public interface IProgressUpdateService
    {
        Task<IEnumerable<ProgressUpdateDto>> GetProgressUpdatesAsync();
        Task<ProgressUpdateDto?> GetProgressUpdateByIdAsync(int id);
        Task<ProgressUpdateDto?> CreateProgressUpdateAsync(CreateProgressUpdateDto dto);
        Task<bool> DeleteProgressUpdateAsync(int id);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
            {
                return null;
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = string.Empty,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    IsActive = user.IsActive
                }
            };
        }
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt,
                    IsActive = u.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UserDto?> CreateUserAsync(CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return null;
            }

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return null;
            }

            var user = new User
            {
                Username = dto.Username.Trim(),
                Email = dto.Email.Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Role = dto.Role,
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            if (dto.Email != null)
            {
                var normalizedEmail = dto.Email.Trim();
                if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail && u.Id != id))
                {
                    return null;
                }

                user.Email = normalizedEmail;
            }

            if (dto.FirstName != null)
            {
                user.FirstName = dto.FirstName.Trim();
            }

            if (dto.LastName != null)
            {
                user.LastName = dto.LastName.Trim();
            }

            if (dto.Role.HasValue)
            {
                user.Role = dto.Role.Value;
            }

            if (dto.IsActive.HasValue)
            {
                user.IsActive = dto.IsActive.Value;
            }

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Milestones)
                .AsNoTracking()
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    OwnerId = p.OwnerId,
                    OwnerName = p.Owner.Username,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    EstimatedCompletionDate = p.EstimatedCompletionDate,
                    Status = p.Status,
                    Location = p.Location,
                    Budget = p.Budget,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    TotalMilestones = p.Milestones.Count,
                    CompletedMilestones = p.Milestones.Count(m => m.Status == MilestoneStatus.Completed),
                    ProgressPercentage = p.Milestones.Count == 0
                        ? 0
                        : (int)Math.Round(p.Milestones.Count(m => m.Status == MilestoneStatus.Completed) * 100.0m / p.Milestones.Count)
                })
                .ToListAsync();
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Milestones)
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    OwnerId = p.OwnerId,
                    OwnerName = p.Owner.Username,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    EstimatedCompletionDate = p.EstimatedCompletionDate,
                    Status = p.Status,
                    Location = p.Location,
                    Budget = p.Budget,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    TotalMilestones = p.Milestones.Count,
                    CompletedMilestones = p.Milestones.Count(m => m.Status == MilestoneStatus.Completed),
                    ProgressPercentage = p.Milestones.Count == 0
                        ? 0
                        : (int)Math.Round(p.Milestones.Count(m => m.Status == MilestoneStatus.Completed) * 100.0m / p.Milestones.Count)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProjectDto?> CreateProjectAsync(CreateProjectDto dto)
        {
            var owner = await _context.Users.FindAsync(dto.OwnerId);
            if (owner == null)
            {
                return null;
            }

            var project = new Project
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                OwnerId = dto.OwnerId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                EstimatedCompletionDate = dto.EstimatedCompletionDate,
                Location = dto.Location.Trim(),
                Budget = dto.Budget,
                Status = ProjectStatus.Planning,
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return await GetProjectByIdAsync(project.Id);
        }

        public async Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return null;
            }

            if (dto.Name != null)
            {
                project.Name = dto.Name.Trim();
            }
            if (dto.Description != null)
            {
                project.Description = dto.Description.Trim();
            }
            if (dto.StartDate.HasValue)
            {
                project.StartDate = dto.StartDate.Value;
            }
            if (dto.EndDate.HasValue)
            {
                project.EndDate = dto.EndDate;
            }
            if (dto.EstimatedCompletionDate.HasValue)
            {
                project.EstimatedCompletionDate = dto.EstimatedCompletionDate;
            }
            if (dto.Status.HasValue)
            {
                project.Status = dto.Status.Value;
            }
            if (dto.Location != null)
            {
                project.Location = dto.Location.Trim();
            }
            if (dto.Budget.HasValue)
            {
                project.Budget = dto.Budget.Value;
            }

            project.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetProjectByIdAsync(id);
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return false;
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class MilestoneService : IMilestoneService
    {
        private readonly ApplicationDbContext _context;

        public MilestoneService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MilestoneDto>> GetMilestonesAsync()
        {
            return await _context.Milestones
                .Include(m => m.Project)
                .Include(m => m.Tasks)
                .AsNoTracking()
                .Select(m => new MilestoneDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    ProjectId = m.ProjectId,
                    ProjectName = m.Project.Name,
                    DueDate = m.DueDate,
                    CompletedAt = m.CompletedAt,
                    Status = m.Status,
                    ProgressPercentage = m.ProgressPercentage,
                    Order = m.Order,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    TotalTasks = m.Tasks.Count,
                    CompletedTasks = m.Tasks.Count(t => t.Status == TaskStatus.Completed)
                })
                .ToListAsync();
        }

        public async Task<MilestoneDto?> GetMilestoneByIdAsync(int id)
        {
            return await _context.Milestones
                .Include(m => m.Project)
                .Include(m => m.Tasks)
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MilestoneDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    ProjectId = m.ProjectId,
                    ProjectName = m.Project.Name,
                    DueDate = m.DueDate,
                    CompletedAt = m.CompletedAt,
                    Status = m.Status,
                    ProgressPercentage = m.ProgressPercentage,
                    Order = m.Order,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    TotalTasks = m.Tasks.Count,
                    CompletedTasks = m.Tasks.Count(t => t.Status == TaskStatus.Completed)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<MilestoneDto?> CreateMilestoneAsync(CreateMilestoneDto dto)
        {
            var project = await _context.Projects.FindAsync(dto.ProjectId);
            if (project == null)
            {
                return null;
            }

            var milestone = new Milestone
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                ProjectId = dto.ProjectId,
                DueDate = dto.DueDate,
                Order = dto.Order,
                Status = MilestoneStatus.NotStarted,
                ProgressPercentage = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.Milestones.Add(milestone);
            await _context.SaveChangesAsync();

            return await GetMilestoneByIdAsync(milestone.Id);
        }

        public async Task<MilestoneDto?> UpdateMilestoneAsync(int id, UpdateMilestoneDto dto)
        {
            var milestone = await _context.Milestones.FindAsync(id);
            if (milestone == null)
            {
                return null;
            }

            if (dto.Name != null)
            {
                milestone.Name = dto.Name.Trim();
            }
            if (dto.Description != null)
            {
                milestone.Description = dto.Description.Trim();
            }
            if (dto.DueDate.HasValue)
            {
                milestone.DueDate = dto.DueDate;
            }
            if (dto.Status.HasValue)
            {
                milestone.Status = dto.Status.Value;
            }
            if (dto.ProgressPercentage.HasValue)
            {
                milestone.ProgressPercentage = dto.ProgressPercentage.Value;
            }
            if (dto.Order.HasValue)
            {
                milestone.Order = dto.Order.Value;
            }

            milestone.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetMilestoneByIdAsync(id);
        }

        public async Task<bool> DeleteMilestoneAsync(int id)
        {
            var milestone = await _context.Milestones.FindAsync(id);
            if (milestone == null)
            {
                return false;
            }

            _context.Milestones.Remove(milestone);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class ProjectTaskService : IProjectTaskService
    {
        private readonly ApplicationDbContext _context;

        public ProjectTaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectTaskDto>> GetTasksAsync()
        {
            return await _context.Tasks
                .Include(t => t.Milestone)
                .Include(t => t.AssignedUser)
                .AsNoTracking()
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    MilestoneId = t.MilestoneId,
                    MilestoneName = t.Milestone.Name,
                    AssignedUserId = t.AssignedUserId,
                    AssignedUserName = t.AssignedUser != null ? t.AssignedUser.Username : null,
                    DueDate = t.DueDate,
                    CompletedAt = t.CompletedAt,
                    Status = t.Status,
                    Priority = t.Priority,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<ProjectTaskDto?> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Milestone)
                .Include(t => t.AssignedUser)
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    MilestoneId = t.MilestoneId,
                    MilestoneName = t.Milestone.Name,
                    AssignedUserId = t.AssignedUserId,
                    AssignedUserName = t.AssignedUser != null ? t.AssignedUser.Username : null,
                    DueDate = t.DueDate,
                    CompletedAt = t.CompletedAt,
                    Status = t.Status,
                    Priority = t.Priority,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProjectTaskDto?> CreateTaskAsync(CreateProjectTaskDto dto)
        {
            var milestone = await _context.Milestones.FindAsync(dto.MilestoneId);
            if (milestone == null)
            {
                return null;
            }

            if (dto.AssignedUserId.HasValue)
            {
                var user = await _context.Users.FindAsync(dto.AssignedUserId.Value);
                if (user == null)
                {
                    return null;
                }
            }

            var task = new ProjectTask
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                MilestoneId = dto.MilestoneId,
                AssignedUserId = dto.AssignedUserId,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                EstimatedHours = dto.EstimatedHours,
                Status = TaskStatus.NotStarted,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(task.Id);
        }

        public async Task<ProjectTaskDto?> UpdateTaskAsync(int id, UpdateProjectTaskDto dto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return null;
            }

            if (dto.Name != null)
            {
                task.Name = dto.Name.Trim();
            }
            if (dto.Description != null)
            {
                task.Description = dto.Description.Trim();
            }
            if (dto.AssignedUserId.HasValue)
            {
                var user = await _context.Users.FindAsync(dto.AssignedUserId.Value);
                if (user == null)
                {
                    return null;
                }
                task.AssignedUserId = dto.AssignedUserId;
            }
            if (dto.DueDate.HasValue)
            {
                task.DueDate = dto.DueDate;
            }
            if (dto.Status.HasValue)
            {
                task.Status = dto.Status.Value;
                if (task.Status == TaskStatus.Completed && !task.CompletedAt.HasValue)
                {
                    task.CompletedAt = DateTime.UtcNow;
                }
            }
            if (dto.Priority.HasValue)
            {
                task.Priority = dto.Priority.Value;
            }
            if (dto.EstimatedHours.HasValue)
            {
                task.EstimatedHours = dto.EstimatedHours.Value;
            }
            if (dto.ActualHours.HasValue)
            {
                task.ActualHours = dto.ActualHours.Value;
            }

            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(id);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsAsync()
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Project)
                .Include(c => c.Milestone)
                .Include(c => c.Task)
                .Include(c => c.Replies)
                .AsNoTracking()
                .ToListAsync();

            return BuildCommentTree(comments)
                .Select(BuildCommentDto)
                .ToList();
        }

        public async Task<CommentDto?> GetCommentByIdAsync(int id)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Project)
                .Include(c => c.Milestone)
                .Include(c => c.Task)
                .AsNoTracking()
                .ToListAsync();

            var comment = comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return null;
            }

            return BuildCommentDto(comment);
        }

        public async Task<CommentDto?> CreateCommentAsync(CreateCommentDto dto)
        {
            if (dto.ProjectId == null && dto.MilestoneId == null && dto.TaskId == null)
            {
                return null;
            }

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return null;
            }

            if (dto.ProjectId.HasValue && !await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId.Value))
            {
                return null;
            }

            if (dto.MilestoneId.HasValue && !await _context.Milestones.AnyAsync(m => m.Id == dto.MilestoneId.Value))
            {
                return null;
            }

            if (dto.TaskId.HasValue && !await _context.Tasks.AnyAsync(t => t.Id == dto.TaskId.Value))
            {
                return null;
            }

            if (dto.ParentCommentId.HasValue && !await _context.Comments.AnyAsync(c => c.Id == dto.ParentCommentId.Value))
            {
                return null;
            }

            var comment = new Comment
            {
                Content = dto.Content.Trim(),
                UserId = dto.UserId,
                ProjectId = dto.ProjectId,
                MilestoneId = dto.MilestoneId,
                TaskId = dto.TaskId,
                ParentCommentId = dto.ParentCommentId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return await GetCommentByIdAsync(comment.Id);
        }

        public async Task<CommentDto?> UpdateCommentAsync(int id, UpdateCommentDto dto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
            }

            comment.Content = dto.Content.Trim();
            comment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetCommentByIdAsync(id);
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return false;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        private static IEnumerable<Comment> BuildCommentTree(IEnumerable<Comment> comments)
        {
            var commentLookup = comments.ToDictionary(c => c.Id);
            foreach (var comment in comments)
            {
                if (comment.ParentCommentId.HasValue && commentLookup.TryGetValue(comment.ParentCommentId.Value, out var parent))
                {
                    parent.Replies.Add(comment);
                }
            }

            return comments.Where(c => c.ParentCommentId == null);
        }

        private static CommentDto BuildCommentDto(Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                UserId = comment.UserId,
                UserName = comment.User.Username,
                ProjectId = comment.ProjectId,
                ProjectName = comment.Project?.Name,
                MilestoneId = comment.MilestoneId,
                MilestoneName = comment.Milestone?.Name,
                TaskId = comment.TaskId,
                TaskName = comment.Task?.Name,
                ParentCommentId = comment.ParentCommentId,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                Replies = comment.Replies.Select(BuildCommentDto).ToList()
            };
        }
    }

    public class ProgressUpdateService : IProgressUpdateService
    {
        private readonly ApplicationDbContext _context;

        public ProgressUpdateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProgressUpdateDto>> GetProgressUpdatesAsync()
        {
            return await _context.ProgressUpdates
                .Include(pu => pu.User)
                .Include(pu => pu.Project)
                .Include(pu => pu.Milestone)
                .Include(pu => pu.Task)
                .AsNoTracking()
                .Select(pu => new ProgressUpdateDto
                {
                    Id = pu.Id,
                    Description = pu.Description,
                    UserId = pu.UserId,
                    UserName = pu.User.Username,
                    ProjectId = pu.ProjectId,
                    ProjectName = pu.Project != null ? pu.Project.Name : null,
                    MilestoneId = pu.MilestoneId,
                    MilestoneName = pu.Milestone != null ? pu.Milestone.Name : null,
                    TaskId = pu.TaskId,
                    TaskName = pu.Task != null ? pu.Task.Name : null,
                    ProgressPercentage = pu.ProgressPercentage,
                    CreatedAt = pu.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ProgressUpdateDto?> GetProgressUpdateByIdAsync(int id)
        {
            return await _context.ProgressUpdates
                .Include(pu => pu.User)
                .Include(pu => pu.Project)
                .Include(pu => pu.Milestone)
                .Include(pu => pu.Task)
                .AsNoTracking()
                .Where(pu => pu.Id == id)
                .Select(pu => new ProgressUpdateDto
                {
                    Id = pu.Id,
                    Description = pu.Description,
                    UserId = pu.UserId,
                    UserName = pu.User.Username,
                    ProjectId = pu.ProjectId,
                    ProjectName = pu.Project != null ? pu.Project.Name : null,
                    MilestoneId = pu.MilestoneId,
                    MilestoneName = pu.Milestone != null ? pu.Milestone.Name : null,
                    TaskId = pu.TaskId,
                    TaskName = pu.Task != null ? pu.Task.Name : null,
                    ProgressPercentage = pu.ProgressPercentage,
                    CreatedAt = pu.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProgressUpdateDto?> CreateProgressUpdateAsync(CreateProgressUpdateDto dto)
        {
            if (dto.ProjectId == null && dto.MilestoneId == null && dto.TaskId == null)
            {
                return null;
            }

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return null;
            }

            if (dto.ProjectId.HasValue && !await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId.Value))
            {
                return null;
            }

            if (dto.MilestoneId.HasValue && !await _context.Milestones.AnyAsync(m => m.Id == dto.MilestoneId.Value))
            {
                return null;
            }

            if (dto.TaskId.HasValue && !await _context.Tasks.AnyAsync(t => t.Id == dto.TaskId.Value))
            {
                return null;
            }

            var progressUpdate = new ProgressUpdate
            {
                Description = dto.Description.Trim(),
                UserId = dto.UserId,
                ProjectId = dto.ProjectId,
                MilestoneId = dto.MilestoneId,
                TaskId = dto.TaskId,
                ProgressPercentage = dto.ProgressPercentage,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProgressUpdates.Add(progressUpdate);
            await _context.SaveChangesAsync();

            return await GetProgressUpdateByIdAsync(progressUpdate.Id);
        }

        public async Task<bool> DeleteProgressUpdateAsync(int id)
        {
            var progressUpdate = await _context.ProgressUpdates.FindAsync(id);
            if (progressUpdate == null)
            {
                return false;
            }

            _context.ProgressUpdates.Remove(progressUpdate);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
