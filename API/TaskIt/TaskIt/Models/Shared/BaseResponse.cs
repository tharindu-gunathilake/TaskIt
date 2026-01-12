namespace TaskIt.Models.Shared
{
    public class BaseResponse<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
        public int? StatusCode { get; init; }

        public static BaseResponse<T> Ok(T data, string? message = null)
        {
            return new BaseResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 200
            };
        }

        public static BaseResponse<T> Fail(string message, int statusCode = 400)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}
