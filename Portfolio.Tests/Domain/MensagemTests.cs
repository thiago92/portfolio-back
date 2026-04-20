using FluentAssertions;
using Portfolio.Domain.Entities;

namespace Portfolio.Tests.Domain
{
    public class MensagemTests
    {
        [Fact]
        public void Mensagem_DeveHerdarDeEntity_ComIdGuid()
        {
            var mensagem = new Mensagem { Texto = "teste" };

            mensagem.Should().BeAssignableTo<Entity>();
            mensagem.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void Mensagem_IdsGeradosDevemSerUnicos()
        {
            var m1 = new Mensagem();
            var m2 = new Mensagem();

            m1.Id.Should().NotBe(m2.Id);
        }
    }
}
