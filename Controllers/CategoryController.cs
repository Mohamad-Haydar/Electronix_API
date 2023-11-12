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
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(ILogger<CategoryController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory()
        {
            return Ok(await _unitOfWork.Category.GetAll());
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] Category Category)
        {
            await _unitOfWork.Category.Add(Category);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPatch("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] Category Category, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Please fill all the nessesaty information" });
            }

            if (Category.Id != id)
            {
                return BadRequest(new { status = "error", message = "Id don't mach" });
            }

            var category = await _unitOfWork.Category.Get(x => x.Id == id);
            if (category == null)
            {
                return BadRequest(new { status = "error", message = "Category don't exists" });
            }
            category.CategoryName = Category.CategoryName;
            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Please fill all the nessesaty information" });
            }

            var category = await _unitOfWork.Category.Get(x => x.Id == id);
            if (category == null)
            {
                return BadRequest(new { status = "error", message = "Category don't exists" });
            }

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            return Ok();
        }

    }
}