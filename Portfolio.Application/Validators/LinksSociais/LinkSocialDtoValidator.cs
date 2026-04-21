using FluentValidation;
using Portfolio.Application.DTOs.LinksSociais;

namespace Portfolio.Application.Validators.LinksSociais
{
    public sealed class LinkSocialDtoValidator : AbstractValidator<LinkSocialDto>
    {
        public LinkSocialDtoValidator()
        {
            RuleFor(x => x.IconeSlug)
                .NotEmpty().WithMessage("O slug do ícone é obrigatório.")
                .MaximumLength(50).WithMessage("O slug do ícone deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("A URL é obrigatória.")
                .MaximumLength(500).WithMessage("A URL deve ter no máximo 500 caracteres.");

            RuleFor(x => x.Tipo)
                .IsInEnum().WithMessage("O tipo informado é inválido.");

            RuleFor(x => x.Ordem)
                .GreaterThanOrEqualTo(0).WithMessage("A ordem não pode ser negativa.");
        }
    }
}
