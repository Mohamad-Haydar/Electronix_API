using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_API.Models;
using Web_API.Repository.IRepository;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
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

        [HttpPatch("UpdateManufacturer")]
        public async Task<IActionResult> UpdateManufacturer([FromBody] Manufacturer Manufacturer, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Please fill all the nessesaty information" });
            }

            if (Manufacturer.Id != id)
            {
                return BadRequest(new { status = "error", message = "Id don't mach" });
            }

            var manufacturer = await _unitOfWork.Manufacturer.Get(x => x.Id == id);
            if (manufacturer == null)
            {
                return BadRequest(new { status = "error", message = "Manufacturer don't exists" });
            }
            manufacturer.ManufacturerName = Manufacturer.ManufacturerName;
            _unitOfWork.Manufacturer.Update(manufacturer);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpDelete("DeleteManufacturer")]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Please fill all the nessesaty information" });
            }
            var manufacturer = await _unitOfWork.Manufacturer.Get(x => x.Id == id);
            if (manufacturer == null)
            {
                return BadRequest(new { status = "error", message = "Manufacturer don't exists" });
            }

            _unitOfWork.Manufacturer.Remove(manufacturer);
            _unitOfWork.Save();
            return Ok();
        }


    }
}