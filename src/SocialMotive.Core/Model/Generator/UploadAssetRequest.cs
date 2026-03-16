namespace SocialMotive.Core.Model.Generator
{
    public class UploadAssetRequest
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] FileData { get; set; } = Array.Empty<byte>();
        public string? Tags { get; set; }
        public bool IsPublic { get; set; } = false;
    }
    
}
