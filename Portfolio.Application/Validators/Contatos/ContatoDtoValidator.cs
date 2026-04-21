using FluentValidation;
using Portfolio.Application.DTOs.Contatos;

namespace Portfolio.Application.Validators.Contatos
{
    public sealed class ContatoDtoValidator : AbstractValidator<ContatoDto>
    {
        public ContatoDtoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");

            RuleFor(x => x.Localizacao)
                .NotEmpty().WithMessage("A localização é obrigatória.")
                .MaximumLength(150).WithMessage("A localização deve ter no máximo 150 caracteres.");

            RuleFor(x => x.Telefone)
                .NotEmpty().WithMessage("O telefone é obrigatório.")
                .MaximumLength(30).WithMessage("O telefone deve ter no máximo 30 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado é inválido.")
                .MaximumLength(200).WithMessage("O e-mail deve ter no máximo 200 caracteres.");
        }
    }
}
