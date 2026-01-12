namespace TaskIt.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<UserTask> Tasks { get; set; } = new List<UserTask>();
    }
}