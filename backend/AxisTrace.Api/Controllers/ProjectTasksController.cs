using AxisTrace.Api.DTOs;
using AxisTrace.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AxisTrace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTasksController : ControllerBase
    {
        private readonly IProjectTaskService _taskService;

        public ProjectTasksController(IProjectTaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetTasks()
        {
            var tasks = await _taskService.GetTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectTaskDto>> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTaskDto>> CreateTask(CreateProjectTaskDto dto)
        {
            var task = await _taskService.CreateTaskAsync(dto);
            if (task == null)
            {
                return BadRequest(new { message = "Milestone or assigned user does not exist." });
            }

            return Ok(task);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProjectTaskDto>> UpdateTask(int id, UpdateProjectTaskDto dto)
        {
            var task = await _taskService.UpdateTaskAsync(id, dto);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
