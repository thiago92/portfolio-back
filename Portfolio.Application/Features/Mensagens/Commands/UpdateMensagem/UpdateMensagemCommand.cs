using MediatR;
using Portfolio.Application.Common;

namespace Portfolio.Application.Features.Mensagens.Commands.UpdateMensagem
{
    public sealed record UpdateMensagemCommand(Guid Id, string Texto) : IRequest<Result>;
}
