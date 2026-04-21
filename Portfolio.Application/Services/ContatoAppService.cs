using FluentValidation;
using MapsterMapper;
using Portfolio.Application.DTOs.Contatos;
using Portfolio.Application.Exceptions;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Services
{
    public class ContatoAppService : IContatoAppService
    {
        private readonly IContatoService _contatoService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IValidator<ContatoDto>> _validators;

        public ContatoAppService(
            IContatoService contatoService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEnumerable<IValidator<ContatoDto>> validators)
        {
            _contatoService = contatoService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validators = validators;
        }

        public async Task<ContatoDto?> GetAsync(CancellationToken cancellationToken = default)
        {
            var contato = await _contatoService.GetUnicoAsync(cancellationToken);
            return contato is null ? null : _mapper.Map<ContatoDto>(contato);
        }

        public async Task<ContatoDto> AtualizarAsync(ContatoDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await ValidarAsync(dto, cancellationToken);

            var existente = await _contatoService.GetUnicoAsync(cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Contato), "singleton");

            _mapper.Map(dto, existente);

            _unitOfWork.Begin();
            _contatoService.Update(existente);
            await _unitOfWork.Confirm(cancellationToken);

            return _mapper.Map<ContatoDto>(existente);
        }

        private async Task ValidarAsync(ContatoDto dto, CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return;

            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(dto, cancellationToken))))
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .Select(f => f.ErrorMessage)
                .ToList();

            if (failures.Count > 0)
                throw new EntityValidationException(failures);
        }
    }
}
