using System.ComponentModel;

namespace Portfolio.Application.DTOs.Habilidades
{
    public sealed record HabilidadeDto
    {
        [ReadOnly(true)]
        public Guid Id { get; init; }

        public string Label { get; init; } = string.Empty;
        public int Valor { get; init; }
        public int Ordem { get; init; }
    }
}
