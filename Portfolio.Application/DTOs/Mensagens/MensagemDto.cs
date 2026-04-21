using System.ComponentModel;

namespace Portfolio.Application.DTOs.Mensagens
{
    public sealed record MensagemDto
    {
        [ReadOnly(true)]
        public Guid Id { get; init; }

        public string Texto { get; init; } = string.Empty;
    }
}
