namespace TaskIt.Models.Responses
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
