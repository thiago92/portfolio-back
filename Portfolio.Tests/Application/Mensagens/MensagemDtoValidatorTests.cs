using FluentAssertions;
using FluentValidation.TestHelper;
using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Application.Validators.Mensagens;

namespace Portfolio.Tests.Application.Mensagens
{
    public class MensagemDtoValidatorTests
    {
        private readonly MensagemDtoValidator _validator = new();

        [Fact]
        public void Deve_FalharQuandoTextoVazio()
        {
            var result = _validator.TestValidate(new MensagemDto { Texto = string.Empty });
            result.ShouldHaveValidationErrorFor(c => c.Texto);
        }

        [Fact]
        public void Deve_FalharQuandoTextoExcede500Caracteres()
        {
            var textoGrande = new string('a', 501);
            var result = _validator.TestValidate(new MensagemDto { Texto = textoGrande });
            result.ShouldHaveValidationErrorFor(c => c.Texto);
        }

        [Fact]
        public void Deve_PassarQuandoTextoValido()
        {
            var result = _validator.TestValidate(new MensagemDto { Texto = "mensagem válida" });
            result.IsValid.Should().BeTrue();
        }
    }
}
