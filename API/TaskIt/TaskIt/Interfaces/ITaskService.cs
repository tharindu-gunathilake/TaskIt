using TaskIt.Models.Requests;
using TaskIt.Models.Responses;
using TaskIt.Models.Shared;

namespace TaskIt.Interfaces
{
    public interface ITaskService
    {
        Task<BaseResponse<TaskListDto>> GetAllTasksAsync(PaginationRequest pagination, Guid userId);
        Task<BaseResponse<UserTaskDto>> GetTaskByIdAsync(Guid id);
        Task<BaseResponse<UserTaskDto>> CreateTaskAsync(CreateTaskDto newTask, Guid userId);
        Task<BaseResponse<UserTaskDto>> UpdateTaskAsync(Guid id, CreateTaskDto updatedTask);
        Task<BaseResponse<bool>> DeleteTaskAsync(Guid id);
    }
}
