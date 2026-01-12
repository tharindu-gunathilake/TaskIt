namespace TaskIt.Models.Responses
{
    public class TaskListDto
    {
        public List<UserTaskDto> UserTasks { get; set; } = [];
        public int TotalCount { get; set; } = 0;
    }
}
