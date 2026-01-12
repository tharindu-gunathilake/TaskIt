using TaskIt.Enums;

namespace TaskIt.Models.Shared
{
    public interface IPaginationRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SearchValue { get; set; }
        public string? SortField { get; set; }
        public string SortOrder { get; set; }
    }

    public class PaginationRequest : IPaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchValue { get; set; }
        public string? SortField { get; set; }
        public string? SortOrder { get; set; } = "asc";
        public TaskStatusEnum? Status { get; set; }
    }
}
