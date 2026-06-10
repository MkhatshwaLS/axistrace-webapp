using AxisTrace.Api.Data;
using AxisTrace.Api.DTOs;
using AxisTrace.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxisTrace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Project)
                .Include(c => c.Milestone)
                .Include(c => c.Task)
                .Include(c => c.Replies)
                .AsNoTracking()
                .ToListAsync();

            var commentDtos = BuildCommentTree(comments)
                .Select(BuildCommentDto)
                .ToList();

            return Ok(commentDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Project)
                .Include(c => c.Milestone)
                .Include(c => c.Task)
                .AsNoTracking()
                .ToListAsync();

            _ = BuildCommentTree(comments);
            var comment = comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(BuildCommentDto(comment));
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto dto)
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

            if (dto.ParentCommentId.HasValue && !await _context.Comments.AnyAsync(c => c.Id == dto.ParentCommentId.Value))
            {
                return BadRequest(new { message = "Parent comment does not exist." });
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

            return await GetComment(comment.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CommentDto>> UpdateComment(int id, UpdateCommentDto dto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.Content = dto.Content.Trim();
            comment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetComment(id);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
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
}
