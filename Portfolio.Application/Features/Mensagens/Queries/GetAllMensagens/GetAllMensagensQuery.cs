using MediatR;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Mensagens;

namespace Portfolio.Application.Features.Mensagens.Queries.GetAllMensagens
{
    public sealed record GetAllMensagensQuery : IRequest<Result<IEnumerable<MensagemDto>>>;
}
