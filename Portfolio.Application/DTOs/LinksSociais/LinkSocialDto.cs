using System.ComponentModel;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.DTOs.LinksSociais
{
    public sealed record LinkSocialDto
    {
        [ReadOnly(true)]
        public Guid Id { get; init; }

        public string IconeSlug { get; init; } = string.Empty;
        public string Url { get; init; } = string.Empty;
        public ETipoLinkSocial Tipo { get; init; }
        public int Ordem { get; init; }
    }
}
