namespace Portfolio.Application.DTOs.Chat
{
    public sealed record ChatRequestDto
    {
        public string Message { get; init; } = string.Empty;
        public IReadOnlyList<ChatMessageDto> History { get; init; } = Array.Empty<ChatMessageDto>();
        public string Language { get; init; } = "pt";
    }
}
