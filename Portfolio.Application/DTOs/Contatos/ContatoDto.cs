using System.ComponentModel;

namespace Portfolio.Application.DTOs.Contatos
{
    public sealed record ContatoDto
    {
        [ReadOnly(true)]
        public Guid Id { get; init; }

        public string Nome { get; init; } = string.Empty;
        public string Localizacao { get; init; } = string.Empty;
        public string Telefone { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }
}
