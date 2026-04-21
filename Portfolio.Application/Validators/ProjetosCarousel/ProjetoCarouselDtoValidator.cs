using FluentValidation;
using Portfolio.Application.DTOs.ProjetosCarousel;

namespace Portfolio.Application.Validators.ProjetosCarousel
{
    public sealed class ProjetoCarouselDtoValidator : AbstractValidator<ProjetoCarouselDto>
    {
        public ProjetoCarouselDtoValidator()
        {
            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("A URL é obrigatória.")
                .MaximumLength(500).WithMessage("A URL deve ter no máximo 500 caracteres.");

            RuleFor(x => x.ImgPath)
                .NotEmpty().WithMessage("O caminho da imagem é obrigatório.")
                .MaximumLength(300).WithMessage("O caminho da imagem deve ter no máximo 300 caracteres.");

            RuleFor(x => x.AltSlug)
                .NotEmpty().WithMessage("O slug do texto alternativo é obrigatório.")
                .MaximumLength(100).WithMessage("O slug deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Ordem)
                .GreaterThanOrEqualTo(0).WithMessage("A ordem não pode ser negativa.");
        }
    }
}
