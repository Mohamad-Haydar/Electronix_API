using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Web_API.Models;
using Web_API.Models.DTO.Request;
using Web_API.Models.DTO.Responce;
using Web_API.Repository.IRepository;
using Web_API.Repository;
using Web_API.Data;

namespace Web_API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProductController : ControllerBase
  {
    private readonly ILogger<ProductController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _db;
    public ProductController(ILogger<ProductController> logger, IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext db)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _db = db;
    }

    #region Get Region

    [HttpGet("GetAllProducts")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllProducts()
    {
      var products = _mapper.Map<List<ProductSummaryVM>>(await _unitOfWork.Product.GetAll(x => x.Manufacturer, y => y.Category));
      return Ok(products);
    }


    [HttpGet("GetProductsOfCategory")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductsOfCategory([Required] string catName)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new { status = "error", message = "Please fill all the necessary information" });
      }
      var products = _mapper.Map<List<ProductSummaryVM>>(
        await _unitOfWork.Product.GetMultiple(x => x.Category.CategoryName.ToLower() == catName.ToLower(),
        M => M.Manufacturer, C => C.Category
      ));
      return Ok(products);
    }

    [HttpGet("GetProductsOfManufacturer")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductsOfManufacturer([Required] int id)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new { status = "error", message = "Please fill all the necessary information" });
      }
      var manufacturer = await _unitOfWork.Manufacturer.Get(x => x.Id == id);
      if (manufacturer == null)
      {
        return NotFound(new { status = "error", message = "Manufacturer Not Found" });
      }
      var products = _mapper.Map<List<ProductSummaryVM>>(
        await _unitOfWork.Product.GetMultiple(x => x.ManufacturerId == id,
        M => M.Manufacturer, C => C.Category
      ));
      return Ok(products);
    }

    [HttpGet("GetOneProduct")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOneProduct(string id)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new { status = "error", message = "Please fill all the necessary information" });
      }
      var product = await _unitOfWork.Product.Get(x => x.Id == id);
      if (product == null)
        return NotFound(new { status = "error", message = "Product Not Found" });

      var manufacturer = await _unitOfWork.Manufacturer.Get(x => x.Id == product.ManufacturerId);
      var category = await _unitOfWork.Category.Get(x => x.Id == product.CategoryId);

      ProductVM ProductVM = new()
      {
        Title = product.Title,
        Description = product.Description,
        Specification = product.Specification,
        Barcode = product.Barcode,
        Discount = product.Discount,
        ImageUrl = product.ImageUrl,
        NummberOfReview = product.NummberOfReview,
        Review = product.Review,
        ManufacturerName = manufacturer.ManufacturerName,
        ManufacturerId = product.ManufacturerId,
        CategoryId = product.CategoryId,
        CategoryName = category.CategoryName,
        Stars5 = product.Stars5,
        Stars4 = product.Stars4,
        Stars3 = product.Stars3,
        Stars2 = product.Stars2,
        Stars1 = product.Stars1
      };

      List<ProductVariantDetailVM> productVariantVMs = new() { };
      var productVariants = await _unitOfWork.ProductVariant.GetMultiple(x => x.ProductId == id);

      // loop over all the variation of this product
      foreach (var item in productVariants)
      {
        // create a productVariantsVMs and add it to the list of productVariantsVMs
        ProductVariantDetailVM productVariantDetailVM = new()
        {
          Id = item.Id,
          Qty = item.Qty,
          Price = item.Price,
          Sku = item.sku
        };

        var productOptionVariants = await _unitOfWork.ProductOptionVariant.GetMultiple(x => x.ProductVariantId == item.Id);
        foreach (var productOptionVariant in productOptionVariants)
        {
          // select the optoin name of these option values in this variant
          var ProductoptionRelation = await _unitOfWork.ProductOption.Get(x => x.Id == productOptionVariant.ProductOptionId);
          var option = await _unitOfWork.Option.Get(x => x.Id == ProductoptionRelation.OptionId);
          if (productVariantDetailVM.optionsValues == null)
            productVariantDetailVM.optionsValues = new Dictionary<string, string>();
          productVariantDetailVM.optionsValues.Add(option.OptionName, productOptionVariant.Value);
        }
        productVariantVMs.Add(productVariantDetailVM);
      }

      ProductVM.ProductVariantDetailVM = productVariantVMs;
      return Ok(ProductVM);
    }

    [HttpGet("GetLast10Item")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLast10Item()
    {
      var products = _mapper.Map<List<ProductSummaryVM>>(await _unitOfWork.Product.GetLatest(10, x => x.Category, x => x.Manufacturer));
      return Ok(products);
    }

    [HttpGet("GetSpecialOffer")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpecialOffer()
    {
      var products = _mapper.Map<List<ProductSummaryVM>>(await _unitOfWork.Product.GetSpecialOffer(10, x => x.Category, x => x.Manufacturer));
      return Ok(products);
    }

    #endregion

    #region  Update Region

    [HttpPatch("UpdateProduct")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,owner")]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductVM updateProductVM, string id)
    {
      if (updateProductVM.Id != id)
      {
        return BadRequest();
      }
      if (!ModelState.IsValid)
      {
        return BadRequest(new RegistrationResponce()
        {
          Errors = new List<string>() { "Invalid payload" },
          Success = false
        });
      }

      var product = await _unitOfWork.Product.Get(x => x.Id == id);

      if (product == null)
      {
        return BadRequest(new RegistrationResponce()
        {
          Errors = new List<string>() { "Product not found" },
          Success = false
        });
      }

      var pov = await _unitOfWork.ProductVariant.GetMultiple(x => x.ProductId == id);

      // update the information of the proudct if they are changed
      product.Title = updateProductVM.Title;
      product.Description = updateProductVM.Description;
      product.Specification = updateProductVM.Specification;
      product.Barcode = updateProductVM.Barcode;
      product.Discount = updateProductVM.Discount;
      product.ImageUrl = updateProductVM.ImageUrl;
      product.CategoryId = updateProductVM.CategoryId;
      product.ManufacturerId = updateProductVM.ManufacturerId;

      // remove all old product variation
      foreach (var item in pov)
      {
        _unitOfWork.ProductOptionVariant.Remove(item.Id);
      }
      _unitOfWork.ProductVariant.Remove(id);
      _unitOfWork.ProductOption.Remove(id);

      var op = await _unitOfWork.Option.GetAll();
      var options = new Dictionary<string, int>();
      foreach (var item in op)
      {
        options.Add(item.OptionName, item.Id);
      }

      // create new product variations
      var res = CreateVariation(updateProductVM.ProductVariantsVMs, product.Id, options);
      if (res == null)
      {
        return BadRequest(new { status = "success", message = "there where an error while creating variatin, please try again" });
      }

      _unitOfWork.Save();
      return Ok();

    }

    #endregion

    #region  Post Region

    [HttpPost("CreateProduct")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,owner")]
    public async Task<IActionResult> CreateProduct([FromBody] AddProductVM ProductVM)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new
        {
          Error = "Please enter all the necessary information to create a product",
          Success = false
        });
      }
      var existingProduct = await _unitOfWork.Product.Get(x => x.Barcode == ProductVM.Barcode || x.Title == ProductVM.Title, false);
      if (existingProduct != null)
      {
        return BadRequest(new
        {
          Error = "Product already exists, please enter another one or shoose to update this product",
          Success = false
        });
      }

      var op = await _unitOfWork.Option.GetAll();
      var options = new Dictionary<string, int>();
      foreach (var item in op)
      {
        options.Add(item.OptionName, item.Id);
      }

      var res = await Createproduct(ProductVM, options);
      if (res)
      {
        _unitOfWork.Save();
        return Ok();
      }

      return BadRequest(new { status = "error", Meesage = "review the file to check the data inside it" });
    }

    [HttpPost("SeedProducts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,owner")]
    public async Task<IActionResult> SeedProducts([FromForm] IFormFile file)
    {
      if (file == null || file.Length == 0)
        return BadRequest("No file uploaded.");

      // Ensure the uploaded file is an image (optional)
      if (file.ContentType.ToLower() != "application/json")
        return BadRequest("Invalid file type. Please upload Json file");


      var op = await _unitOfWork.Option.GetAll();
      var options = new Dictionary<string, int>();
      foreach (var item in op)
      {
        options.Add(item.OptionName, item.Id);
      }

      // Read the content of the JSON file
      using (var streamReader = new StreamReader(file.OpenReadStream()))
      {
        var jsonContent = await streamReader.ReadToEndAsync();
        try
        {
          var jsonObject = JsonConvert.DeserializeObject<List<AddProductVM>>(jsonContent);
          foreach (var item in jsonObject)
          {
            var res = await Createproduct(item, options);
            if (!res)
            {
              return BadRequest();
            }
          }
          _unitOfWork.Save();
        }
        catch (System.Text.Json.JsonException)
        {
          return BadRequest("Invalid JSON format.");
        }
      }
      return Ok();
    }

    [HttpPost("PostReview")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")]
    public async Task<IActionResult> PostReview([FromBody] ReviewDTO ReviewDTO)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new
        {
          Error = "Please fill all the user and the product information to rate the product",
          Success = false
        });
      }

      var product = await _unitOfWork.Product.Get(x => x.Id == ReviewDTO.ProductId);
      if (product != null)
      {
        var oldUserReview = await _unitOfWork.UserProductReview.Get(x => x.UserId == ReviewDTO.UserId && x.ProductId == ReviewDTO.ProductId);

        if (oldUserReview == null)
        {
          // we need to add a new review to this user
          UserProductReview userProductReview = new()
          {
            UserId = ReviewDTO.UserId,
            ProductId = ReviewDTO.ProductId,
            Review = ReviewDTO.Review
          };
          await _unitOfWork.UserProductReview.Add(userProductReview);

          PropertyInfo property = product.GetType().GetProperty("Stars" + ReviewDTO.Review);

          int currentValue = (int)property.GetValue(product);
          property.SetValue(product, currentValue + 1);

          var newReview = (product.NummberOfReview * product.Review + ReviewDTO.Review) / (product.NummberOfReview + 1);
          product.Review = newReview;
          product.NummberOfReview++;
        }
        else
        {
          // we need to update the review of this user
          // 1- substract one from the old review numbers
          PropertyInfo property = product.GetType().GetProperty("Stars" + oldUserReview.Review);
          int currentValue = (int)property.GetValue(product);
          property.SetValue(product, currentValue - 1);

          // 2- add one to this new review
          property = product.GetType().GetProperty("Stars" + ReviewDTO.Review);
          currentValue = (int)property.GetValue(product);
          property.SetValue(product, currentValue + 1);

          // 3- change the review of the product
          var newReview = (product.NummberOfReview * product.Review + ReviewDTO.Review - oldUserReview.Review) / product.NummberOfReview;
          product.Review = newReview;
          oldUserReview.Review = ReviewDTO.Review;
        }
        _unitOfWork.Save();
        return Ok();
      }

      return BadRequest(new
      {
        Error = "Product does not exists in the reviews table, please run the refrecher and retry the review",
        Success = false
      });
    }


    #endregion

    #region Delete Region
    [HttpDelete("DeleteProduct")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,owner")]
    public async Task<IActionResult> DeleteProduct(string Id)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new { status = "error", message = "Please enter an ID of a product to make the remove" });
      }
      var product = await _unitOfWork.Product.Get(x => x.Id == Id);
      if (product == null)
      {
        return BadRequest(new { status = "error", message = "product don't exists" });
      }

      _unitOfWork.Product.Remove(product);
      _unitOfWork.Save();

      return Ok(new { status = "success", message = "Product deleted successfully" });
    }

    #endregion
    private async Task<bool> CreateVariation(ICollection<ProductVariantsVM> productVariantsVMs, string id, Dictionary<string, int> options)
    {

      // var option = await _unitOfWork.Option.GetAll();

      foreach (var item in productVariantsVMs)
      {
        // add new productVariant to the productvariant table
        string pvId = Guid.NewGuid().ToString();
        ProductVariant productVariant = new()
        {
          Id = pvId,
          ProductId = id,
          sku = item.Sku,
          Qty = item.Qty,
          Price = item.Price
        };
        await _unitOfWork.ProductVariant.Add(productVariant);

        // add all the option value to this specific productvariant relation
        if (item.OptionsValues != null)
        {
          foreach (var OV in item.OptionsValues)
          {
            try
            {
              string poId = Guid.NewGuid().ToString();
              // var singleOptoin = option.FirstOrDefault(x => x.OptionName == OV.Key).Id;
              ProductOption productOption = new()
              {
                Id = poId,
                ProductId = id,
                // OptionId = OV.Key == "ramStorage" ? 1 : 2
                OptionId = options[OV.Key]
              };

              ProductOptionVariant productOptionVariant = new()
              {
                ProductOptionId = poId,
                ProductVariantId = pvId,
                Value = OV.Value
              };
              await _unitOfWork.ProductOption.Add(productOption);
              await _unitOfWork.ProductOptionVariant.Add(productOptionVariant);
            }
            catch (Exception ex)
            {
              // Log or handle the exception as needed
              Console.WriteLine($"An error occurred in the loop: {ex.Message}");
              // You might want to rethrow the exception if you want to stop processing after an error
              // throw;
            }
          }
        }
        else
        {
          return false;
        }

      }
      return true;
    }

    private async Task<bool> Createproduct(AddProductVM ProductVM, Dictionary<string, int> options)
    {
      // create a new product
      Product product = new()
      {
        Id = Guid.NewGuid().ToString(),
        Title = ProductVM.Title,
        Description = ProductVM.Description,
        Specification = ProductVM.Specification,
        Barcode = ProductVM.Barcode,
        Discount = ProductVM.Discount,
        ImageUrl = ProductVM.ImageUrl,
        CategoryId = ProductVM.CategoryId,
        AddedDate = DateTime.Now,
        ConcurrencyStamp = "",
        ManufacturerId = ProductVM.ManufacturerId,
        NummberOfReview = ProductVM.NumberOfReview,
        Review = ProductVM.Review,
        Stars1 = ProductVM.Star1,
        Stars2 = ProductVM.Star2,
        Stars3 = ProductVM.Star3,
        Stars4 = ProductVM.Star4,
        Stars5 = ProductVM.Star5,
      };

      var result = await _unitOfWork.Product.Add(product);

      if (result && ProductVM.ProductVariantsVMs != null)
      {
        var res = CreateVariation(ProductVM.ProductVariantsVMs, product.Id, options);
        if (res == null)
        {
          return false;
        }
        return true;

      }
      return false;
    }


    #region EntitiesAproach

    // [HttpGet("productEntity")]
    // public async Task<IActionResult> GetOneEntity(string id)
    // {

    //   var productTask = await _db.Products.ToModelAsync();

    //   return Ok(productTask);
    // }

    #endregion


  }
}