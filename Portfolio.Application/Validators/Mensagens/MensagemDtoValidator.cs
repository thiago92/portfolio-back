using FluentValidation;
using Portfolio.Application.DTOs.Mensagens;

namespace Portfolio.Application.Validators.Mensagens
{
    public sealed class MensagemDtoValidator : AbstractValidator<MensagemDto>
    {
        public MensagemDtoValidator()
        {
            RuleFor(x => x.Texto)
                .NotEmpty().WithMessage("O texto é obrigatório.")
                .MaximumLength(500).WithMessage("O texto deve ter no máximo 500 caracteres.");
        }
    }
}
