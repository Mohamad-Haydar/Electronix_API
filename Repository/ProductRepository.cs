using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_API.Data;
using Web_API.Models;
using Web_API.Models.DTO.Request;
using Web_API.Models.DTO.Responce;
using Web_API.Repository.IRepository;

namespace Web_API.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;

        public ProductRepository(ApplicationDbContext db, ILogger logger) : base(db, logger)
        {
            _db = db;
            _logger = logger;
        }

        public override async Task<bool> Add([FromBody] Product product)
        {
            await dbSet.AddAsync(product);
            return true;
        }

        public async Task AddProductRange(List<AddProductVM> products)
        {
            foreach (var ProductVM in products)
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

                await _db.Products.AddAsync(product);

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
                    await _db.ProductVariants.AddAsync(productVariant);

                    // add all the option value to this specific productvariant relation
                    foreach (var OV in item.OptionsValues)
                    {
                        string poId = Guid.NewGuid().ToString();
                        var option = await _db.Options.FirstOrDefaultAsync(x => x.OptionName == OV.Key);
                        ProductOption productOption = new()
                        {
                            Id = poId,
                            ProductId = product.Id,
                            OptionId = option.Id
                        };

                        ProductOptionVariant productOptionVariant = new()
                        {
                            ProductOptionId = poId,
                            ProductVariantId = pvId,
                            Value = OV.Value
                        };
                        await _db.ProductOptions.AddAsync(productOption);
                        await _db.ProductOptionVariantS.AddAsync(productOptionVariant);
                    }
                }

            }

        }

        public async Task<IEnumerable<Product>> GetLatest(int number, params Expression<Func<Product, object>>[] includes)
        {
            var products = dbSet.OrderByDescending(p => p.AddedDate).Take(number);
            foreach (var include in includes)
            {
                products = products.Include(include);
            }
            return await products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetSpecialOffer(int number, params Expression<Func<Product, object>>[] includes)
        {
            var products = dbSet.Where(x => x.Discount != 0).OrderByDescending(p => p.AddedDate).Take(number);
            foreach (var include in includes)
            {
                products = products.Include(include);
            }
            return await products.ToListAsync();
        }

    }
}