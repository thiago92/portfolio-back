using System.ComponentModel;

namespace Portfolio.Application.DTOs.ProjetosCarousel
{
    public sealed record ProjetoCarouselDto
    {
        [ReadOnly(true)]
        public Guid Id { get; init; }

        public string Url { get; init; } = string.Empty;
        public string ImgPath { get; init; } = string.Empty;
        public string AltSlug { get; init; } = string.Empty;
        public int Ordem { get; init; }
    }
}
