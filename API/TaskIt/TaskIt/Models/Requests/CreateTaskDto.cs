using TaskIt.Enums;

namespace TaskIt.Models.Requests
{
    public class CreateTaskDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;
    }
}
