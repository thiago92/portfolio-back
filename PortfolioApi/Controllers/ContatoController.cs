using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.DTOs.Contatos;
using Portfolio.Application.Interface;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ContatoController : ControllerBase
    {
        private readonly IContatoAppService _service;

        public ContatoController(IContatoAppService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var dto = await _service.GetAsync(cancellationToken);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put([FromBody] ContatoDto dto, CancellationToken cancellationToken)
            => Ok(await _service.AtualizarAsync(dto, cancellationToken));
    }
}
