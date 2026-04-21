using System.ComponentModel;

namespace Portfolio.Application.DTOs.Trabalhos
{
    public sealed record TrabalhoDto
    {
        [ReadOnly(true)]
        public Guid Id { get; init; }

        public string TituloSlug { get; init; } = string.Empty;
        public string TextoSlug { get; init; } = string.Empty;
        public string ImgPath { get; init; } = string.Empty;
        public string TextoBotaoSlug { get; init; } = string.Empty;
        public string Href { get; init; } = string.Empty;
        public string TooltipSlug { get; init; } = string.Empty;
        public int Ordem { get; init; }
    }
}
