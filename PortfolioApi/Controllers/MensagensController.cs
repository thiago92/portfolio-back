using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Application.Features.Mensagens.Commands.CreateMensagem;
using Portfolio.Application.Features.Mensagens.Commands.DeleteMensagem;
using Portfolio.Application.Features.Mensagens.Commands.UpdateMensagem;
using Portfolio.Application.Features.Mensagens.Queries.GetAllMensagens;
using Portfolio.Application.Features.Mensagens.Queries.GetMensagemById;
using PortfolioApi.Extensions;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class MensagensController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MensagensController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllMensagensQuery(), cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMensagemByIdQuery(id), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMensagemDto dto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateMensagemCommand(dto.Texto), cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
                : result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMensagemDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id) return BadRequest();
            var result = await _mediator.Send(new UpdateMensagemCommand(dto.Id, dto.Texto), cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteMensagemCommand(id), cancellationToken);
            return result.ToActionResult();
        }
    }
}
