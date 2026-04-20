using MediatR;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Mensagens;

namespace Portfolio.Application.Features.Mensagens.Queries.GetMensagemById
{
    public sealed record GetMensagemByIdQuery(Guid Id) : IRequest<Result<MensagemDto>>;
}
