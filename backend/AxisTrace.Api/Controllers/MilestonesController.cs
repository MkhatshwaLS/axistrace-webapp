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
    public class MilestonesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MilestonesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MilestoneDto>>> GetMilestones()
        {
            var milestones = await _context.Milestones
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

            return Ok(milestones);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MilestoneDto>> GetMilestone(int id)
        {
            var milestone = await _context.Milestones
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

            if (milestone == null)
            {
                return NotFound();
            }

            return Ok(milestone);
        }

        [HttpPost]
        public async Task<ActionResult<MilestoneDto>> CreateMilestone(CreateMilestoneDto dto)
        {
            var project = await _context.Projects.FindAsync(dto.ProjectId);
            if (project == null)
            {
                return BadRequest(new { message = "Parent project does not exist." });
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

            return await GetMilestone(milestone.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MilestoneDto>> UpdateMilestone(int id, UpdateMilestoneDto dto)
        {
            var milestone = await _context.Milestones.FindAsync(id);
            if (milestone == null)
            {
                return NotFound();
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

            return await GetMilestone(id);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMilestone(int id)
        {
            var milestone = await _context.Milestones.FindAsync(id);
            if (milestone == null)
            {
                return NotFound();
            }

            _context.Milestones.Remove(milestone);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
