using Portfolio.Application.DTOs.Auth;

namespace Portfolio.Application.Interface
{
    public interface IAuthAppService
    {
        Task<TokenDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    }
}
