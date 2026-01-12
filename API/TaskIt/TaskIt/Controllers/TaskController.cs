using Microsoft.AspNetCore.Mvc;
using TaskIt.Domain;
using TaskIt.Interfaces;
using TaskIt.Models.Requests;
using TaskIt.Models.Shared;

namespace TaskIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] PaginationRequest pagination, [FromHeader(Name = "x-user-id")] Guid userId)
        {
            var response = await _taskService.GetAllTasksAsync(pagination, userId);

            return StatusCode(
                response.StatusCode ?? StatusCodes.Status200OK,
                response
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTaskById([FromRoute] Guid id)
        {
            var response = await _taskService.GetTaskByIdAsync(id);
            return StatusCode(
                response.StatusCode ?? StatusCodes.Status200OK,
                response
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto newTask, [FromHeader(Name = "x-user-id")] Guid userId)
        {
            var response = await _taskService.CreateTaskAsync(newTask, userId);

            return StatusCode(
                response.StatusCode ?? StatusCodes.Status200OK,
                response
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] CreateTaskDto updatedTask)
        {
            var response = await _taskService.UpdateTaskAsync(id, updatedTask);

            return StatusCode(
                response.StatusCode ?? StatusCodes.Status200OK,
                response
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid id)
        {
            var response = await _taskService.DeleteTaskAsync(id);

            return StatusCode(
                response.StatusCode ?? StatusCodes.Status200OK,
                response
            );
        }
    }
}
