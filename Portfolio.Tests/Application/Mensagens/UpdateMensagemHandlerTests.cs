using FluentAssertions;
using Moq;
using Portfolio.Application.Common;
using Portfolio.Application.Features.Mensagens.Commands.UpdateMensagem;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Tests.Application.Mensagens
{
    public class UpdateMensagemHandlerTests
    {
        [Fact]
        public async Task Handle_DeveRetornarNotFound_QuandoMensagemNaoExiste()
        {
            var repositoryMock = new Mock<IRepository<Mensagem>>();
            repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Mensagem?)null);

            var handler = new UpdateMensagemHandler(repositoryMock.Object);
            var command = new UpdateMensagemCommand(Guid.NewGuid(), "novo texto");

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.NotFound);
            repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Mensagem>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DeveAtualizarTexto_QuandoMensagemExiste()
        {
            var id = Guid.NewGuid();
            var existente = new Mensagem { Id = id, Texto = "antigo" };

            var repositoryMock = new Mock<IRepository<Mensagem>>();
            repositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existente);

            var handler = new UpdateMensagemHandler(repositoryMock.Object);
            var command = new UpdateMensagemCommand(id, "novo");

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            existente.Texto.Should().Be("novo");
            repositoryMock.Verify(r => r.UpdateAsync(existente, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
