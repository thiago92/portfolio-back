using FluentAssertions;
using FluentValidation.TestHelper;
using Portfolio.Application.Features.Mensagens.Commands.CreateMensagem;

namespace Portfolio.Tests.Application.Mensagens
{
    public class CreateMensagemValidatorTests
    {
        private readonly CreateMensagemValidator _validator = new();

        [Fact]
        public void Deve_FalharQuandoTextoVazio()
        {
            var result = _validator.TestValidate(new CreateMensagemCommand(string.Empty));
            result.ShouldHaveValidationErrorFor(c => c.Texto);
        }

        [Fact]
        public void Deve_FalharQuandoTextoExcede500Caracteres()
        {
            var textoGrande = new string('a', 501);
            var result = _validator.TestValidate(new CreateMensagemCommand(textoGrande));
            result.ShouldHaveValidationErrorFor(c => c.Texto);
        }

        [Fact]
        public void Deve_PassarQuandoTextoValido()
        {
            var result = _validator.TestValidate(new CreateMensagemCommand("mensagem válida"));
            result.IsValid.Should().BeTrue();
        }
    }
}
