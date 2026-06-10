using AxisTrace.Api.Data;
using AxisTrace.Api.DTOs;
using AxisTrace.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxisTrace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressUpdatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProgressUpdatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgressUpdateDto>>> GetProgressUpdates()
        {
            var updates = await _context.ProgressUpdates
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

            return Ok(updates);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProgressUpdateDto>> GetProgressUpdate(int id)
        {
            var progressUpdate = await _context.ProgressUpdates
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

            if (progressUpdate == null)
            {
                return NotFound();
            }

            return Ok(progressUpdate);
        }

        [HttpPost]
        public async Task<ActionResult<ProgressUpdateDto>> CreateProgressUpdate(CreateProgressUpdateDto dto)
        {
            if (dto.ProjectId == null && dto.MilestoneId == null && dto.TaskId == null)
            {
                return BadRequest(new { message = "A project, milestone, or task reference is required." });
            }

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User does not exist." });
            }

            if (dto.ProjectId.HasValue && !await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId.Value))
            {
                return BadRequest(new { message = "Project does not exist." });
            }

            if (dto.MilestoneId.HasValue && !await _context.Milestones.AnyAsync(m => m.Id == dto.MilestoneId.Value))
            {
                return BadRequest(new { message = "Milestone does not exist." });
            }

            if (dto.TaskId.HasValue && !await _context.Tasks.AnyAsync(t => t.Id == dto.TaskId.Value))
            {
                return BadRequest(new { message = "Task does not exist." });
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

            return await GetProgressUpdate(progressUpdate.Id);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProgressUpdate(int id)
        {
            var progressUpdate = await _context.ProgressUpdates.FindAsync(id);
            if (progressUpdate == null)
            {
                return NotFound();
            }

            _context.ProgressUpdates.Remove(progressUpdate);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
