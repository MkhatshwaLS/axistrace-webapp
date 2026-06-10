using AxisTrace.Api.Data;
using AxisTrace.Api.DTOs;
using AxisTrace.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxisTrace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _context.Projects
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

            return Ok(projects);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _context.Projects
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

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto dto)
        {
            var owner = await _context.Users.FindAsync(dto.OwnerId);
            if (owner == null)
            {
                return BadRequest(new { message = "Owner user does not exist." });
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

            var createdProject = await _context.Projects
                .Include(p => p.Owner)
                .AsNoTracking()
                .Where(p => p.Id == project.Id)
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
                    TotalMilestones = 0,
                    CompletedMilestones = 0,
                    ProgressPercentage = 0
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetProject), new { id = createdProject.Id }, createdProject);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProjectDto>> UpdateProject(int id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
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

            return await GetProject(id);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
