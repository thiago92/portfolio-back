using FluentAssertions;
using Moq;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;
using Portfolio.Domain.Services;

namespace Portfolio.Tests.Domain
{
    public class MensagemServiceTests
    {
        [Fact]
        public async Task GetByIdAsync_DeveDelegarAoRepositorio()
        {
            var id = Guid.NewGuid();
            var esperado = new Mensagem { Id = id, Texto = "x" };

            var repo = new Mock<IMensagemRepository>();
            repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(esperado);

            var service = new MensagemService(repo.Object);

            var result = await service.GetByIdAsync(id);

            result.Should().BeSameAs(esperado);
            repo.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Add_DeveDelegarAoRepositorio()
        {
            var repo = new Mock<IMensagemRepository>();
            var service = new MensagemService(repo.Object);
            var entity = new Mensagem { Texto = "x" };

            service.Add(entity);

            repo.Verify(r => r.Add(entity), Times.Once);
        }

        [Fact]
        public void Update_DeveDelegarAoRepositorio()
        {
            var repo = new Mock<IMensagemRepository>();
            var service = new MensagemService(repo.Object);
            var entity = new Mensagem { Texto = "x" };

            service.Update(entity);

            repo.Verify(r => r.Update(entity), Times.Once);
        }

        [Fact]
        public void Remove_DeveDelegarAoRepositorio()
        {
            var repo = new Mock<IMensagemRepository>();
            var service = new MensagemService(repo.Object);
            var entity = new Mensagem { Texto = "x" };

            service.Remove(entity);

            repo.Verify(r => r.Remove(entity), Times.Once);
        }
    }
}
