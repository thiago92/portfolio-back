using MapsterMapper;
using MediatR;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Features.Mensagens.Queries.GetAllMensagens
{
    public sealed class GetAllMensagensHandler : IRequestHandler<GetAllMensagensQuery, Result<IEnumerable<MensagemDto>>>
    {
        private readonly IRepository<Mensagem> _repository;
        private readonly IMapper _mapper;

        public GetAllMensagensHandler(IRepository<Mensagem> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<MensagemDto>>> Handle(GetAllMensagensQuery request, CancellationToken cancellationToken)
        {
            var mensagens = await _repository.GetAllAsync(cancellationToken);
            return Result.Success(_mapper.Map<IEnumerable<MensagemDto>>(mensagens));
        }
    }
}
