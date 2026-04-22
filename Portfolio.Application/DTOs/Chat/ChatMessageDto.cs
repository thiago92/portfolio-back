namespace Portfolio.Application.DTOs.Chat
{
    public sealed record ChatMessageDto
    {
        public string Role { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
    }
}
