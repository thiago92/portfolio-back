using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController<T> : ControllerBase where T : EntityBase
    {
        protected readonly IAppService<T> _service;

        public BaseController(IAppService<T> service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult Get(Guid id) => Ok(_service.Get(id));

        [HttpPost]
        public IActionResult Post(T entity)
        {
            _service.Create(entity);
            return Ok(entity);
        }

        [HttpPut]
        public IActionResult Put(T entity)
        {
            _service.Update(entity);
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _service.Delete(id);
            return Ok();
        }
    }
}
