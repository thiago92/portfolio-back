using FluentValidation;

namespace Portfolio.Application.Features.Mensagens.Commands.UpdateMensagem
{
    public sealed class UpdateMensagemValidator : AbstractValidator<UpdateMensagemCommand>
    {
        public UpdateMensagemValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Texto)
                .NotEmpty().WithMessage("O texto é obrigatório.")
                .MaximumLength(500).WithMessage("O texto deve ter no máximo 500 caracteres.");
        }
    }
}
