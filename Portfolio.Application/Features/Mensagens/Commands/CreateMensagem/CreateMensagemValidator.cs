using FluentValidation;

namespace Portfolio.Application.Features.Mensagens.Commands.CreateMensagem
{
    public sealed class CreateMensagemValidator : AbstractValidator<CreateMensagemCommand>
    {
        public CreateMensagemValidator()
        {
            RuleFor(x => x.Texto)
                .NotEmpty().WithMessage("O texto é obrigatório.")
                .MaximumLength(500).WithMessage("O texto deve ter no máximo 500 caracteres.");
        }
    }
}
