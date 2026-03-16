namespace SocialMotive.Core.Model
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
}
