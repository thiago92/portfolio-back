using Portfolio.Application.DTOs.Chat;

namespace Portfolio.Application.Interface
{
    public interface IChatAppService
    {
        Task<ChatResponseDto> SendMessageAsync(ChatRequestDto dto, CancellationToken cancellationToken = default);
    }
}
