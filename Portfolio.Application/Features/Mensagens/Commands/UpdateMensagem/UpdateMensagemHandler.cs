using MediatR;
using Portfolio.Application.Common;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Features.Mensagens.Commands.UpdateMensagem
{
    public sealed class UpdateMensagemHandler : IRequestHandler<UpdateMensagemCommand, Result>
    {
        private readonly IRepository<Mensagem> _repository;

        public UpdateMensagemHandler(IRepository<Mensagem> repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(UpdateMensagemCommand request, CancellationToken cancellationToken)
        {
            var mensagem = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (mensagem is null)
                return Result.Failure(Error.NotFound("Mensagem.NotFound", $"Mensagem {request.Id} não encontrada."));

            mensagem.Texto = request.Texto;
            await _repository.UpdateAsync(mensagem, cancellationToken);
            return Result.Success();
        }
    }
}
