using Mapster;
using Portfolio.Application.DTOs.ProjetosCarousel;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings
{
    public sealed class ProjetoCarouselMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ProjetoCarousel, ProjetoCarouselDto>();

            config.NewConfig<ProjetoCarouselDto, ProjetoCarousel>()
                .Ignore(dest => dest.Id);
        }
    }
}
