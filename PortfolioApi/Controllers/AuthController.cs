using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.DTOs.Auth;
using Portfolio.Application.Interface;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthAppService _authService;

        public AuthController(IAuthAppService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
            => Ok(await _authService.LoginAsync(dto, cancellationToken));
    }
}
