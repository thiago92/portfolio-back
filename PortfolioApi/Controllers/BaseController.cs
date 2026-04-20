using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController<T> : ControllerBase where T : Entity
    {
        protected readonly IAppService<T> _service;

        public BaseController(IAppService<T> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
            => Ok(await _service.GetAllAsync(cancellationToken));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _service.GetAsync(id, cancellationToken);
            return entity is null ? NotFound() : Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Post(T entity, CancellationToken cancellationToken)
        {
            await _service.CreateAsync(entity, cancellationToken);
            return Ok(entity);
        }

        [HttpPut]
        public async Task<IActionResult> Put(T entity, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(entity, cancellationToken);
            return Ok(entity);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return Ok();
        }
    }
}
