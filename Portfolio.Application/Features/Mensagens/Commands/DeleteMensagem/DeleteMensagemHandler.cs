using MediatR;
using Portfolio.Application.Common;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Features.Mensagens.Commands.DeleteMensagem
{
    public sealed class DeleteMensagemHandler : IRequestHandler<DeleteMensagemCommand, Result>
    {
        private readonly IRepository<Mensagem> _repository;

        public DeleteMensagemHandler(IRepository<Mensagem> repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(DeleteMensagemCommand request, CancellationToken cancellationToken)
        {
            var mensagem = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (mensagem is null)
                return Result.Failure(Error.NotFound("Mensagem.NotFound", $"Mensagem {request.Id} não encontrada."));

            await _repository.DeleteAsync(request.Id, cancellationToken);
            return Result.Success();
        }
    }
}
