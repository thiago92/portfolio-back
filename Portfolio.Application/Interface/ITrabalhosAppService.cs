using Portfolio.Application.DTOs.Trabalhos;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interface
{
    public interface ITrabalhosAppService : IAppService<Trabalho, TrabalhoDto>
    {
    }
}
