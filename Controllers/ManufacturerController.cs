using Microsoft.AspNetCore.Mvc;
using Web_API.Models;
using Web_API.Models.Variants;
using Web_API.Models.ViewModels;
using Web_API.Repository.IRepository;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManufacturerController : ControllerBase
    {
        private readonly ILogger<ManufacturerController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ManufacturerController(ILogger<ManufacturerController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAllManufacturer")]
        public async Task<IActionResult> GetAllManufacturer()
        {
            return Ok(await _unitOfWork.Manufacturer.GetAll());
        }

        [HttpPost("AddManufacturer")]
        public async Task<IActionResult> AddManufacturer([FromBody] Manufacturer Manufacturer)
        {
            await _unitOfWork.Manufacturer.Add(Manufacturer);
            _unitOfWork.Save();
            return Ok();
        }
    }
}