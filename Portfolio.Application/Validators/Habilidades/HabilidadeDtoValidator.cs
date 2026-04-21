using FluentValidation;
using Portfolio.Application.DTOs.Habilidades;

namespace Portfolio.Application.Validators.Habilidades
{
    public sealed class HabilidadeDtoValidator : AbstractValidator<HabilidadeDto>
    {
        public HabilidadeDtoValidator()
        {
            RuleFor(x => x.Label)
                .NotEmpty().WithMessage("O label é obrigatório.")
                .MaximumLength(50).WithMessage("O label deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Valor)
                .InclusiveBetween(0, 100).WithMessage("O valor deve estar entre 0 e 100.");

            RuleFor(x => x.Ordem)
                .GreaterThanOrEqualTo(0).WithMessage("A ordem não pode ser negativa.");
        }
    }
}
