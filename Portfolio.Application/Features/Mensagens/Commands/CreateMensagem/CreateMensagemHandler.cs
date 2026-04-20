using MapsterMapper;
using MediatR;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Features.Mensagens.Commands.CreateMensagem
{
    public sealed class CreateMensagemHandler : IRequestHandler<CreateMensagemCommand, Result<MensagemDto>>
    {
        private readonly IRepository<Mensagem> _repository;
        private readonly IMapper _mapper;

        public CreateMensagemHandler(IRepository<Mensagem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MensagemDto>> Handle(CreateMensagemCommand request, CancellationToken cancellationToken)
        {
            var mensagem = new Mensagem { Texto = request.Texto };
            await _repository.AddAsync(mensagem, cancellationToken);
            return _mapper.Map<MensagemDto>(mensagem);
        }
    }
}
