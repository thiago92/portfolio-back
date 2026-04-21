using Mapster;
using Portfolio.Application.DTOs.Habilidades;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings
{
    public sealed class HabilidadeMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Habilidade, HabilidadeDto>();

            config.NewConfig<HabilidadeDto, Habilidade>()
                .Ignore(dest => dest.Id);
        }
    }
}
