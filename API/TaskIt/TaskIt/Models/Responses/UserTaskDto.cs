using TaskIt.Domain;
using TaskIt.Enums;

namespace TaskIt.Models.Responses
{
    public class UserTaskDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Guid UserId { get; set; }
        public TaskStatusEnum Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public static UserTaskDto FromDomain(UserTask task)
        {
            return new UserTaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                UserId = task.UserId,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                DeletedAt = task.DeletedAt
            };
        }
    }
}
