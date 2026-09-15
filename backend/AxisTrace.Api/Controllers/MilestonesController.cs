using AxisTrace.Api.DTOs;
using AxisTrace.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AxisTrace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MilestonesController : ControllerBase
    {
        private readonly IMilestoneService _milestoneService;

        public MilestonesController(IMilestoneService milestoneService)
        {
            _milestoneService = milestoneService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MilestoneDto>>> GetMilestones()
        {
            var milestones = await _milestoneService.GetMilestonesAsync();
            return Ok(milestones);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MilestoneDto>> GetMilestone(int id)
        {
            var milestone = await _milestoneService.GetMilestoneByIdAsync(id);
            if (milestone == null)
            {
                return NotFound();
            }

            return Ok(milestone);
        }

        [HttpPost]
        public async Task<ActionResult<MilestoneDto>> CreateMilestone(CreateMilestoneDto dto)
        {
            var milestone = await _milestoneService.CreateMilestoneAsync(dto);
            if (milestone == null)
            {
                return BadRequest(new { message = "Parent project does not exist." });
            }

            return Ok(milestone);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MilestoneDto>> UpdateMilestone(int id, UpdateMilestoneDto dto)
        {
            var milestone = await _milestoneService.UpdateMilestoneAsync(id, dto);
            if (milestone == null)
            {
                return NotFound();
            }

            return Ok(milestone);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMilestone(int id)
        {
            var deleted = await _milestoneService.DeleteMilestoneAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
