using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Moq;
using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Application.Exceptions;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Tests.Application.Mensagens
{
    public class MensagensAppServiceTests
    {
        private static MensagensAppService BuildService(
            Mock<IMensagemService> domainServiceMock,
            Mock<IUnitOfWork> unitOfWorkMock,
            Mock<IMapper> mapperMock,
            IEnumerable<IValidator<MensagemDto>>? validators = null)
        {
            return new MensagensAppService(
                domainServiceMock.Object,
                unitOfWorkMock.Object,
                mapperMock.Object,
                validators ?? Array.Empty<IValidator<MensagemDto>>());
        }

        private static Mock<IValidator<MensagemDto>> ValidatorMock(bool valid, string? errorMessage = null)
        {
            var result = valid
                ? new ValidationResult()
                : new ValidationResult(new[] { new ValidationFailure("Texto", errorMessage ?? "erro") });

            var mock = new Mock<IValidator<MensagemDto>>();
            mock.Setup(v => v.ValidateAsync(It.IsAny<MensagemDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return mock;
        }

        [Fact]
        public async Task CreateAsync_DeveGerarNovoIdIgnorandoOEnviadoNoDto()
        {
            var idClienteMalicioso = Guid.NewGuid();
            var domain = new Mock<IMensagemService>();
            var uow = new Mock<IUnitOfWork>();
            var mapper = new Mock<IMapper>();

            Mensagem? adicionada = null;
            domain.Setup(d => d.Add(It.IsAny<Mensagem>()))
                .Callback<Mensagem>(m => adicionada = m);

            mapper.Setup(m => m.Map<Mensagem>(It.IsAny<MensagemDto>()))
                .Returns<MensagemDto>(d => new Mensagem { Id = d.Id, Texto = d.Texto });
            mapper.Setup(m => m.Map<MensagemDto>(It.IsAny<Mensagem>()))
                .Returns<Mensagem>(m => new MensagemDto { Id = m.Id, Texto = m.Texto });

            var service = BuildService(domain, uow, mapper);
            var dto = new MensagemDto { Id = idClienteMalicioso, Texto = "olá mundo" };

            var result = await service.CreateAsync(dto, CancellationToken.None);

            adicionada.Should().NotBeNull();
            adicionada!.Id.Should().NotBe(Guid.Empty);
            adicionada.Id.Should().NotBe(idClienteMalicioso, "o backend deve gerar um Id novo e ignorar o que veio no body");
            adicionada.Texto.Should().Be("olá mundo");
            result.Id.Should().Be(adicionada.Id);
            domain.Verify(d => d.Add(It.IsAny<Mensagem>()), Times.Once);
            uow.Verify(u => u.Begin(), Times.Once);
            uow.Verify(u => u.Confirm(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarEntityValidation_QuandoInvalido()
        {
            var domain = new Mock<IMensagemService>();
            var uow = new Mock<IUnitOfWork>();
            var mapper = new Mock<IMapper>();

            var service = BuildService(
                domain, uow, mapper,
                new[] { ValidatorMock(valid: false, "Texto obrigatório").Object });

            var act = () => service.CreateAsync(new MensagemDto { Texto = string.Empty }, CancellationToken.None);

            await act.Should().ThrowAsync<EntityValidationException>();
            domain.Verify(d => d.Add(It.IsAny<Mensagem>()), Times.Never);
            uow.Verify(u => u.Confirm(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarEntityNotFound_QuandoInexistente()
        {
            var domain = new Mock<IMensagemService>();
            domain.Setup(d => d.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Mensagem?)null);

            var service = BuildService(domain, new Mock<IUnitOfWork>(), new Mock<IMapper>());

            var act = () => service.UpdateAsync(Guid.NewGuid(), new MensagemDto { Texto = "novo" }, CancellationToken.None);

            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizarTextoPreservandoIdDaUrl()
        {
            var id = Guid.NewGuid();
            var existente = new Mensagem { Id = id, Texto = "antigo" };

            var domain = new Mock<IMensagemService>();
            domain.Setup(d => d.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existente);

            var uow = new Mock<IUnitOfWork>();
            var mapper = new Mock<IMapper>();

            mapper.Setup(m => m.Map(It.IsAny<MensagemDto>(), It.IsAny<Mensagem>()))
                .Returns<MensagemDto, Mensagem>((dto, dest) =>
                {
                    dest.Texto = dto.Texto;
                    return dest;
                });
            mapper.Setup(m => m.Map<MensagemDto>(It.IsAny<Mensagem>()))
                .Returns<Mensagem>(m => new MensagemDto { Id = m.Id, Texto = m.Texto });

            var service = BuildService(domain, uow, mapper);
            var dto = new MensagemDto { Id = Guid.NewGuid(), Texto = "novo" };

            var result = await service.UpdateAsync(id, dto, CancellationToken.None);

            result.Id.Should().Be(id, "o Id da URL é autoritativo, o do body é ignorado");
            result.Texto.Should().Be("novo");
            existente.Id.Should().Be(id);
            existente.Texto.Should().Be("novo");
            domain.Verify(d => d.Update(existente), Times.Once);
            uow.Verify(u => u.Confirm(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveRemoverEConfirmarUoW()
        {
            var id = Guid.NewGuid();
            var existente = new Mensagem { Id = id, Texto = "x" };

            var domain = new Mock<IMensagemService>();
            domain.Setup(d => d.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existente);

            var uow = new Mock<IUnitOfWork>();
            var service = BuildService(domain, uow, new Mock<IMapper>());

            await service.DeleteAsync(id, CancellationToken.None);

            domain.Verify(d => d.Remove(existente), Times.Once);
            uow.Verify(u => u.Confirm(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveLancarEntityNotFound_QuandoInexistente()
        {
            var domain = new Mock<IMensagemService>();
            domain.Setup(d => d.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Mensagem?)null);

            var service = BuildService(domain, new Mock<IUnitOfWork>(), new Mock<IMapper>());

            var act = () => service.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task GetAllAsync_DeveMapearEntidadesParaDtos()
        {
            var mensagens = new[]
            {
                new Mensagem { Id = Guid.NewGuid(), Texto = "a" },
                new Mensagem { Id = Guid.NewGuid(), Texto = "b" }
            };

            var domain = new Mock<IMensagemService>();
            domain.Setup(d => d.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mensagens);

            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<IEnumerable<MensagemDto>>(mensagens))
                .Returns(mensagens.Select(m => new MensagemDto { Id = m.Id, Texto = m.Texto }));

            var service = BuildService(domain, new Mock<IUnitOfWork>(), mapper);

            var result = await service.GetAllAsync(CancellationToken.None);

            result.Should().HaveCount(2);
        }
    }
}
