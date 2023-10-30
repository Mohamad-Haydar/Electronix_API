using Microsoft.AspNetCore.Mvc;
using Web_API.Models;
using Web_API.Repository.IRepository;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OptionController : ControllerBase
    {
        private readonly ILogger<OptionController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public OptionController(ILogger<OptionController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAllOption")]
        public async Task<IActionResult> GetAllOption()
        {
            return Ok(await _unitOfWork.Option.GetAll());
        }

        [HttpPost("AddOption")]
        public async Task<IActionResult> AddOption([FromBody] Option Option)
        {
            await _unitOfWork.Option.Add(Option);
            _unitOfWork.Save();
            return Ok();
        }
    }
}