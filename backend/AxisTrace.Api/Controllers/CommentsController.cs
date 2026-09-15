using AxisTrace.Api.DTOs;
using AxisTrace.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AxisTrace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
        {
            var comments = await _commentService.GetCommentsAsync();
            return Ok(comments);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto dto)
        {
            var comment = await _commentService.CreateCommentAsync(dto);
            if (comment == null)
            {
                return BadRequest(new { message = "A valid project, milestone, task, or parent comment reference is required." });
            }

            return Ok(comment);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CommentDto>> UpdateComment(int id, UpdateCommentDto dto)
        {
            var comment = await _commentService.UpdateCommentAsync(id, dto);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var deleted = await _commentService.DeleteCommentAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
