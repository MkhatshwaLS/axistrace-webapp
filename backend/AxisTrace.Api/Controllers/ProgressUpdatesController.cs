using AxisTrace.Api.DTOs;
using AxisTrace.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AxisTrace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressUpdatesController : ControllerBase
    {
        private readonly IProgressUpdateService _progressUpdateService;

        public ProgressUpdatesController(IProgressUpdateService progressUpdateService)
        {
            _progressUpdateService = progressUpdateService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgressUpdateDto>>> GetProgressUpdates()
        {
            var updates = await _progressUpdateService.GetProgressUpdatesAsync();
            return Ok(updates);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProgressUpdateDto>> GetProgressUpdate(int id)
        {
            var progressUpdate = await _progressUpdateService.GetProgressUpdateByIdAsync(id);
            if (progressUpdate == null)
            {
                return NotFound();
            }

            return Ok(progressUpdate);
        }

        [HttpPost]
        public async Task<ActionResult<ProgressUpdateDto>> CreateProgressUpdate(CreateProgressUpdateDto dto)
        {
            var progressUpdate = await _progressUpdateService.CreateProgressUpdateAsync(dto);
            if (progressUpdate == null)
            {
                return BadRequest(new { message = "A valid project, milestone, or task reference is required." });
            }

            return Ok(progressUpdate);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProgressUpdate(int id)
        {
            var deleted = await _progressUpdateService.DeleteProgressUpdateAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
