using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_API.Data;
using Web_API.Models.Variants;
using Web_API.Models.ViewModels;
using Web_API.Repository.IRepository;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VariantsController : ControllerBase
    {
        private readonly ILogger<VariantsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public VariantsController(IUnitOfWork unitOfWork, ILogger<VariantsController> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #region Get Variants Region
        [HttpGet("GetColor")]
        public async Task<IActionResult> GetColor()
        {
            return Ok(await _unitOfWork.Color.GetAll());
        }

        [HttpGet("GetSize")]
        public async Task<IActionResult> GetSize()
        {
            return Ok(await _unitOfWork.Size.GetAll());
        }

        [HttpGet("GetMemoryStorage")]
        public async Task<IActionResult> GetMemoryStorage()
        {
            return Ok(await _unitOfWork.MemoryStorage.GetAll());
        }
        #endregion

        #region Post Variant Region

        [HttpPost("AddColor")]
        public async Task<IActionResult> AddColor([FromBody] Color color)
        {
            await _unitOfWork.Color.Add(color);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPost("AddSize")]
        public async Task<IActionResult> AddSize([FromBody] Size size)
        {
            await _unitOfWork.Size.Add(size);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPost("AddMemoryStorage")]
        public async Task<IActionResult> AddMemoryStorage([FromBody] MemoryStorage memoryStorage)
        {
            await _unitOfWork.MemoryStorage.Add(memoryStorage);
            _unitOfWork.Save();
            return Ok();
        }
        #endregion
    }
}