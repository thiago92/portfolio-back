namespace Portfolio.Application.DTOs.Auth
{
    public sealed record TokenDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
    }
}
