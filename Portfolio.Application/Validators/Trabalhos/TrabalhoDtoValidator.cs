using FluentValidation;
using Portfolio.Application.DTOs.Trabalhos;

namespace Portfolio.Application.Validators.Trabalhos
{
    public sealed class TrabalhoDtoValidator : AbstractValidator<TrabalhoDto>
    {
        public TrabalhoDtoValidator()
        {
            RuleFor(x => x.TituloSlug)
                .NotEmpty().WithMessage("O slug do título é obrigatório.")
                .MaximumLength(100).WithMessage("O slug do título deve ter no máximo 100 caracteres.");

            RuleFor(x => x.TextoSlug)
                .NotEmpty().WithMessage("O slug do texto é obrigatório.")
                .MaximumLength(100).WithMessage("O slug do texto deve ter no máximo 100 caracteres.");

            RuleFor(x => x.ImgPath)
                .NotEmpty().WithMessage("O caminho da imagem é obrigatório.")
                .MaximumLength(300).WithMessage("O caminho da imagem deve ter no máximo 300 caracteres.");

            RuleFor(x => x.TextoBotaoSlug)
                .NotEmpty().WithMessage("O slug do texto do botão é obrigatório.")
                .MaximumLength(100).WithMessage("O slug do texto do botão deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Href)
                .NotEmpty().WithMessage("O href é obrigatório.")
                .MaximumLength(500).WithMessage("O href deve ter no máximo 500 caracteres.");

            RuleFor(x => x.TooltipSlug)
                .NotEmpty().WithMessage("O slug do tooltip é obrigatório.")
                .MaximumLength(100).WithMessage("O slug do tooltip deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Ordem)
                .GreaterThanOrEqualTo(0).WithMessage("A ordem não pode ser negativa.");
        }
    }
}
