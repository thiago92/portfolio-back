using FluentAssertions;
using MapsterMapper;
using Moq;
using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Application.Features.Mensagens.Commands.CreateMensagem;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Tests.Application.Mensagens
{
    public class CreateMensagemHandlerTests
    {
        [Fact]
        public async Task Handle_DeveCriarMensagemERetornarDto()
        {
            var repositoryMock = new Mock<IRepository<Mensagem>>();
            var mapperMock = new Mock<IMapper>();

            Mensagem? capturada = null;
            repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Mensagem>(), It.IsAny<CancellationToken>()))
                .Callback<Mensagem, CancellationToken>((m, _) => capturada = m)
                .Returns(Task.CompletedTask);

            mapperMock
                .Setup(m => m.Map<MensagemDto>(It.IsAny<Mensagem>()))
                .Returns<Mensagem>(m => new MensagemDto(m.Id, m.Texto));

            var handler = new CreateMensagemHandler(repositoryMock.Object, mapperMock.Object);
            var command = new CreateMensagemCommand("olá mundo");

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Texto.Should().Be("olá mundo");
            capturada.Should().NotBeNull();
            capturada!.Texto.Should().Be("olá mundo");
            repositoryMock.Verify(r => r.AddAsync(It.IsAny<Mensagem>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
