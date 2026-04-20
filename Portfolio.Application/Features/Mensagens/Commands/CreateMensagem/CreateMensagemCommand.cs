using MediatR;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Mensagens;

namespace Portfolio.Application.Features.Mensagens.Commands.CreateMensagem
{
    public sealed record CreateMensagemCommand(string Texto) : IRequest<Result<MensagemDto>>;
}
