using Mapster;
using Portfolio.Application.DTOs.Contatos;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings
{
    public sealed class ContatoMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Contato, ContatoDto>();

            config.NewConfig<ContatoDto, Contato>()
                .Ignore(dest => dest.Id);
        }
    }
}
