namespace SocialMotive.WebApp.Models
{
    /// <summary>
    /// Generic interface for standardized API response envelope
    /// </summary>
    public interface IResponseDto<T>
    {
        bool Success { get; set; }
        string Message { get; set; }
        T? Data { get; set; }
        IEnumerable<string>? Errors { get; set; }
    }

    /// <summary>
    /// Generic standardized API response
    /// </summary>
    public class Response<T> : IResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public Response()
        {
        }

        public Response(bool success, string message, T? data = default, IEnumerable<string>? errors = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
        }

        /// <summary>
        /// Create a successful response
        /// </summary>
        public static Response<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new Response<T>(true, message, data);
        }

        /// <summary>
        /// Create a failed response
        /// </summary>
        public static Response<T> FailureResponse(string message, IEnumerable<string>? errors = null)
        {
            return new Response<T>(false, message, errors: errors);
        }
    }

    /// <summary>
    /// Non-generic standardized API response
    /// </summary>
    public class Response : IResponseDto<object?>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public Response()
        {
        }

        public Response(bool success, string message, object? data = null, IEnumerable<string>? errors = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
        }

        /// <summary>
        /// Create a successful response
        /// </summary>
        public static Response SuccessResponse(object? data = null, string message = "Operation successful")
        {
            return new Response(true, message, data);
        }

        /// <summary>
        /// Create a failed response
        /// </summary>
        public static Response FailureResponse(string message, IEnumerable<string>? errors = null)
        {
            return new Response(false, message, errors: errors);
        }
    }

    /// <summary>
    /// Paginated response for list queries
    /// </summary>
    public class PaginatedResponse<T> : Response<IEnumerable<T>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PaginatedResponse()
        {
        }

        public PaginatedResponse(
            IEnumerable<T> data,
            int pageNumber,
            int pageSize,
            int totalCount,
            string message = "Operation successful")
        {
            Success = true;
            Message = message;
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        /// <summary>
        /// Create a successful paginated response
        /// </summary>
        public static PaginatedResponse<T> SuccessResponse(
            IEnumerable<T> data,
            int pageNumber,
            int pageSize,
            int totalCount,
            string message = "Operation successful")
        {
            return new PaginatedResponse<T>(data, pageNumber, pageSize, totalCount, message);
        }

        /// <summary>
        /// Create a failed paginated response
        /// </summary>
        public static PaginatedResponse<T> FailureResponse(string message, IEnumerable<string>? errors = null)
        {
            return new PaginatedResponse<T>
            {
                Success = false,
                Message = message,
                Data = Enumerable.Empty<T>(),
                Errors = errors,
                PageNumber = 1,
                PageSize = 0,
                TotalCount = 0
            };
        }
    }
}
