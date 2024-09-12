using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,owner")]
        public async Task<IActionResult> AddOption([FromBody] Option Option)
        {
            await _unitOfWork.Option.Add(Option);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPatch("UpdateOption")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,owner")]
        public async Task<IActionResult> UpdateOption([FromBody] Option Option, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Please fill all the nessesaty information" });
            }

            if (Option.Id != id)
            {
                return BadRequest(new { status = "error", message = "Id don't mach" });
            }

            var option = await _unitOfWork.Option.Get(x => x.Id == id);
            if (option == null)
            {
                return BadRequest(new { status = "error", message = "Option don't exists" });
            }
            option.OptionName = Option.OptionName;
            _unitOfWork.Option.Update(option);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpDelete("DeleteOption")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,owner")]
        public async Task<IActionResult> DeleteOption(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Please fill all the nessesaty information" });
            }

            var option = await _unitOfWork.Option.Get(x => x.Id == id);
            if (option == null)
            {
                return BadRequest(new { status = "error", message = "Option don't exists" });
            }

            _unitOfWork.Option.Remove(option);
            _unitOfWork.Save();
            return Ok();
        }


    }
}