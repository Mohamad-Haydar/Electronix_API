using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web_API.Models;
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
    private readonly IMapper _mapper;

    public ProductController(ILogger<ProductController> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _mapper = mapper;
    }

    [HttpGet("GetAllProducts")]
    public async Task<IActionResult> GetAllProducts()
    {
      var products = _mapper.Map<List<ProductSummaryVM>>(await _unitOfWork.Product.GetAll());

      return Ok(products);
    }

    [HttpGet("GetProductsOfCategory")]
    public async Task<IActionResult> GetProductsOfCategory(int id)
    {
      var products = await _unitOfWork.Product.GetMultiple(x => x.CategoryId == id);
      return Ok(products);
    }

    [HttpGet("GetProductsOfManufacturer")]
    public async Task<IActionResult> GetProductsOfManufacturer(int id)
    {
      var products = await _unitOfWork.Product.GetMultiple(x => x.ManufacturerId == id);
      return Ok(products);
    }


    [HttpGet("GetOneProduct")]
    public async Task<IActionResult> GetOneProduct(string id)
    {
      if (!ModelState.IsValid)
        return BadRequest();
      var product = await _unitOfWork.Product.GetProduct(id);
      if (product == null)
        return NotFound();

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
        Manufacturer = manufacturer.ManufacturerName,
        Category = category.CategoryName
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

    [HttpPost("CreateProduct")]
    public async Task<IActionResult> CreateProduct([FromBody] AddProductVM ProductVM)
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
        ConcurrencyStamp = "",
        ManufacturerId = ProductVM.ManufacturerId,
        NummberOfReview = 0,
        Review = 0
      };

      await _unitOfWork.Product.Add(product);

      foreach (var item in ProductVM.ProductVariantsVMs)
      {
        // add new productVariant to the productvariant table
        string pvId = Guid.NewGuid().ToString();
        ProductVariant productVariant = new()
        {
          Id = pvId,
          ProductId = product.Id,
          sku = item.Sku,
          Qty = item.Qty,
          Price = item.Price
        };
        await _unitOfWork.ProductVariant.Add(productVariant);

        // add all the option value to this specific productvariant relation
        foreach (var OV in item.optionsValues)
        {
          string poId = Guid.NewGuid().ToString();
          ProductOption productOption = new()
          {
            Id = poId,
            ProductId = product.Id,
            OptionId = (int)OV.Key
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
      }
      _unitOfWork.Save();


      var options = new JsonSerializerOptions
      {
        ReferenceHandler = ReferenceHandler.Preserve
      };

      var json = JsonSerializer.Serialize(product, options);

      return Ok();
    }

  }
}
