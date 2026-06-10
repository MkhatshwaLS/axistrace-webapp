using AxisTrace.Api.Data;
using AxisTrace.Api.DTOs;
using AxisTrace.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskStatus = AxisTrace.Api.Models.TaskStatus;

namespace AxisTrace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetTasks()
        {
            var tasks = await _context.Tasks
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

            return Ok(tasks);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectTaskDto>> GetTask(int id)
        {
            var task = await _context.Tasks
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

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTaskDto>> CreateTask(CreateProjectTaskDto dto)
        {
            var milestone = await _context.Milestones.FindAsync(dto.MilestoneId);
            if (milestone == null)
            {
                return BadRequest(new { message = "Milestone does not exist." });
            }

            if (dto.AssignedUserId.HasValue)
            {
                var user = await _context.Users.FindAsync(dto.AssignedUserId.Value);
                if (user == null)
                {
                    return BadRequest(new { message = "Assigned user does not exist." });
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

            return await GetTask(task.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProjectTaskDto>> UpdateTask(int id, UpdateProjectTaskDto dto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
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
                    return BadRequest(new { message = "Assigned user does not exist." });
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

            return await GetTask(id);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
