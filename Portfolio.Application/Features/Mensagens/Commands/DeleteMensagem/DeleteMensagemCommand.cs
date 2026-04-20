using MediatR;
using Portfolio.Application.Common;

namespace Portfolio.Application.Features.Mensagens.Commands.DeleteMensagem
{
    public sealed record DeleteMensagemCommand(Guid Id) : IRequest<Result>;
}
