using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.DTOs.Chat;
using Portfolio.Application.Interface;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public sealed class ChatController : ControllerBase
    {
        private readonly IChatAppService _service;

        public ChatController(IChatAppService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequestDto dto, CancellationToken cancellationToken)
            => Ok(await _service.SendMessageAsync(dto, cancellationToken));
    }
}
