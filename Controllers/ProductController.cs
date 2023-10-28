using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Web_API.Models;
using Web_API.Models.Variants;
using Web_API.Models.ViewModels;
using Web_API.Repository.IRepository;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(ILogger<ProductController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] AddProductVM ProductVM)
        {
            var id = Guid.NewGuid().ToString();
            Product product = new()
            {
                Id = id,
                Title = ProductVM.Title,
                Description = ProductVM.Description,
                Barcode = ProductVM.Barcode,
                Discount = ProductVM.Discount,
                ImageUrl = ProductVM.ImageUrl,
                CategoryId = ProductVM.CategoryId,
                ManufacturerId = ProductVM.ManufacturerId,
                ConcurrencyStamp = ""
            };
            List<ProductVariant> productVariants = new();
            foreach (var item in ProductVM.ProductVariantsVMs)
            {
                var color = await _unitOfWork.Color.Get(x => x.ColorName == item.Color);
                var memoryStorage = await _unitOfWork.MemoryStorage.Get(x => x.Memory == item.Memory && x.Storage == item.Storage);
                var size = await _unitOfWork.Size.Get(x => x.SizeNumber == item.Size);


                ProductVariant pv = new()
                {
                    ProductId = id,
                    Qty = item.Qty,
                    Price = item.Price,
                    sku = Guid.NewGuid().ToString()
                };
                productVariants.Add(pv);

                PVColor pVColor = new()
                {
                    Color = color,
                    ProductVariant = pv
                };
                PVMemoryStorage pVMemoryStorage = new()
                {
                    MemoryStorage = memoryStorage,
                    ProductVariant = pv
                };
                PVSize pVSize = new()
                {
                    Size = size,
                    ProductVariant = pv
                };

                await _unitOfWork.PVColor.Add(pVColor);
                await _unitOfWork.PVSize.Add(pVSize);
                await _unitOfWork.PVMemoryStorage.Add(pVMemoryStorage);

                await _unitOfWork.ProductVariant.Add(pv);
            }

            product.ProductVariants = productVariants;
            await _unitOfWork.Product.Add(product);
            _unitOfWork.Save();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var json = JsonSerializer.Serialize(product, options);

            return Ok(json);
        }

    }
}

/*
{
  "title": "phone",
  "description": "The best in its category",
  "barcode": 1020304,
  "discount": 0,
  "imageUrl": "",
  "categoryId": 1,
  "manufacturerId": 1,
  "productVariantsVMs": [
    {
      "qty": 20,
      "memory": 8,
      "storage": 128,
      "color": "Black",
      "size": 6.9,
      "price": 210
    },
{
      "qty": 5,
      "memory": 8,
      "storage": 256,
      "color": "blue",
      "size": 6.9,
      "price": 240
    }
  ]
}
*/