using SocialMotive.Core.Model;
namespace SocialMotive.Core.Model
{
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

        /// <summary>
        /// Create a response with list of data items
        /// </summary>
        public static Response<T> ListResponse(T data, int totalCount, string message = "Retrieved successfully")
        {
            return new Response<T>(true, message, data);
        }
    }
}
