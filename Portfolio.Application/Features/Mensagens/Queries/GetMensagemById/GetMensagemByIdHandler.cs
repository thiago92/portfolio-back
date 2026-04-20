using MapsterMapper;
using MediatR;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Features.Mensagens.Queries.GetMensagemById
{
    public sealed class GetMensagemByIdHandler : IRequestHandler<GetMensagemByIdQuery, Result<MensagemDto>>
    {
        private readonly IRepository<Mensagem> _repository;
        private readonly IMapper _mapper;

        public GetMensagemByIdHandler(IRepository<Mensagem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MensagemDto>> Handle(GetMensagemByIdQuery request, CancellationToken cancellationToken)
        {
            var mensagem = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (mensagem is null)
                return Error.NotFound("Mensagem.NotFound", $"Mensagem {request.Id} não encontrada.");

            return _mapper.Map<MensagemDto>(mensagem);
        }
    }
}
