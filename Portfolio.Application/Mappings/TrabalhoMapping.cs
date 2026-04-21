using Mapster;
using Portfolio.Application.DTOs.Trabalhos;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings
{
    public sealed class TrabalhoMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Trabalho, TrabalhoDto>();

            config.NewConfig<TrabalhoDto, Trabalho>()
                .Ignore(dest => dest.Id);
        }
    }
}
