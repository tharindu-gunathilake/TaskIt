using TaskIt.Enums;

namespace TaskIt.Domain
{
    public class UserTask
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public TaskStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}