namespace Portfolio.Application.DTOs.Auth
{
    public sealed record LoginDto
    {
        public string Email { get; init; } = string.Empty;
        public string Senha { get; init; } = string.Empty;
    }
}
