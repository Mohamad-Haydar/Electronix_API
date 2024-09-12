using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_API.Models;
using Web_API.Repository.IRepository;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        // private readonly ILogger<CategoryController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory()
        {
            return Ok(await _unitOfWork.Category.GetAll());
        }

        [HttpPost("AddCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,owner")]
        public async Task<IActionResult> AddCategory([FromBody] Category Category)
        {
            var existsCategory = await _unitOfWork.Category.Get(x => x.CategoryName == Category.CategoryName);
            if (existsCategory != null)
            {
                return BadRequest(new { status = "error", message = "Category already exists" });
            }
            await _unitOfWork.Category.Add(Category);
            _unitOfWork.Save();
            return Ok(new { status = "success", message = "Category is Added successfully" });
        }

        [HttpPatch("UpdateCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,owner")]
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
            // chekc if the new name that the admin gave is already exists
            var existsCategory = await _unitOfWork.Category.Get(x => x.CategoryName == Category.CategoryName);
            if (existsCategory != null)
            {
                return BadRequest(new { status = "error", message = "Category already exists" });
            }
            category.CategoryName = Category.CategoryName;
            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            return Ok(new { status = "success", message = "Category updated successfully" });
        }

        [HttpDelete("DeleteCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,owner")]
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
            return Ok(new { status = "success", message = "Category deleted successfully" });
        }

    }
}