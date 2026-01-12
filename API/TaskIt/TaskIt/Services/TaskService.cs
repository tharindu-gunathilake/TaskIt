using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskIt.Data;
using TaskIt.Domain;
using TaskIt.Interfaces;
using TaskIt.Models.Requests;
using TaskIt.Models.Responses;
using TaskIt.Models.Shared;

namespace TaskIt.Services
{
    public class TaskService: ITaskService
    {
        private readonly TaskItDbContext _dbContext;

        public TaskService(TaskItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BaseResponse<UserTaskDto>> CreateTaskAsync(CreateTaskDto newTask, Guid userId)
        {
            try
            {
                var task = new UserTask
                {
                    Id = Guid.NewGuid(),
                    Name = newTask.Name,
                    Description = newTask.Description,
                    UserId = userId,
                    Status = newTask.Status,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.UserTasks.Add(task);
                var result = _dbContext.SaveChanges();

                return result > 0
                    ? BaseResponse<UserTaskDto>.Ok(UserTaskDto.FromDomain(task), "User Task Created Successfully")
                    : BaseResponse<UserTaskDto>.Fail("No User Task Created", StatusCodes.Status500InternalServerError);
            }
            catch (Exception e)
            {
                Console.WriteLine($"CreateTaskAsync Error : {e.Message}");
                return BaseResponse<UserTaskDto>.Fail("Failed to Create User Task", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<BaseResponse<bool>> DeleteTaskAsync(Guid id)
        {
            try
            {
                var existingTask = await _dbContext.UserTasks.FindAsync(id);
                if (existingTask != null)
                {
                    existingTask.DeletedAt = DateTime.UtcNow;
                    var updatedTask = _dbContext.UserTasks.Update(existingTask);
                    var result = _dbContext.SaveChanges();

                    return result > 0
                        ? BaseResponse<bool>.Ok(true, "User Task Deleted Successfully")
                        : BaseResponse<bool>.Fail("No User Task Deleted", StatusCodes.Status500InternalServerError);
                }

                return BaseResponse<bool>.Fail("User Task Not Found", StatusCodes.Status404NotFound);
            }
            catch (Exception e)
            {
                Console.WriteLine($"DeleteTaskAsync Error : {e.Message}");
                return BaseResponse<bool>.Fail("Failed to Delete User Task", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<BaseResponse<TaskListDto>> GetAllTasksAsync(PaginationRequest pagination, Guid userId)
        {
            try
            {
                var taskListResponse = new TaskListDto();
                var query = _dbContext.UserTasks.AsQueryable();

                query = query.Where(t => t.DeletedAt == null && t.UserId == userId);

                if (!string.IsNullOrEmpty(pagination.SearchValue))
                {
                    query = query.Where(t => t.Name.ToLower().Contains(pagination.SearchValue.ToLower())
                    || (t.Description != null && t.Description.ToLower().Contains(pagination.SearchValue.ToLower())));
                }
                if (pagination.Status != null)
                {
                    query = query.Where(t => t.Status == pagination.Status);
                }
                if (!string.IsNullOrEmpty(pagination.SortField))
                {
                    query = pagination.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(e => EF.Property<object>(e, pagination.SortField))
                        : query.OrderBy(e => EF.Property<object>(e, pagination.SortField));
                }
                else
                {
                    query = query.OrderByDescending(t => t.CreatedAt);
                }
                var tasks = await query
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToArrayAsync();
                var totalCount = await query.CountAsync();

                var taskDtos = tasks.Select(UserTaskDto.FromDomain).ToArray();

                taskListResponse.UserTasks = taskDtos.ToList();
                taskListResponse.TotalCount = totalCount;

                return BaseResponse<TaskListDto>.Ok(taskListResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetAllTasksAsync Error : {e.Message}");
                return BaseResponse<TaskListDto>.Fail("Failed to Retrieve User Tasks", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<BaseResponse<UserTaskDto>> GetTaskByIdAsync(Guid id)
        {
            try
            {
                var existingTask = await _dbContext.UserTasks.FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

                return existingTask != null
                    ? BaseResponse<UserTaskDto>.Ok(UserTaskDto.FromDomain(existingTask), "User Task Retrieved Successfully")
                    : BaseResponse<UserTaskDto>.Fail("User Task Not Found", StatusCodes.Status404NotFound);
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetTaskByIdAsync Error : {e.Message}");
                return BaseResponse<UserTaskDto>.Fail("Failed to Retrieve User Task", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<BaseResponse<UserTaskDto>> UpdateTaskAsync(Guid id, CreateTaskDto updatedTask)
        {
            try
            {
                var existingTask = await _dbContext.UserTasks.FindAsync(id);
                if (existingTask != null)
                {
                    existingTask.Name = updatedTask.Name;
                    existingTask.Description = updatedTask.Description;
                    existingTask.Status = updatedTask.Status;
                    existingTask.UpdatedAt = DateTime.UtcNow;
                    _dbContext.UserTasks.Update(existingTask);
                    var result = _dbContext.SaveChanges();

                    return result > 0
                    ? BaseResponse<UserTaskDto>.Ok(UserTaskDto.FromDomain(existingTask), "User Task Updated Successfully")
                    : BaseResponse<UserTaskDto>.Fail("No User Task Updated", StatusCodes.Status500InternalServerError);
                }

                return BaseResponse<UserTaskDto>.Fail("User Task Not Found", StatusCodes.Status404NotFound);
            }
            catch (Exception e)
            {
                Console.WriteLine($"UpdateTaskAsync Error : {e.Message}");
                return BaseResponse<UserTaskDto>.Fail("Failed to Update User Task", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
