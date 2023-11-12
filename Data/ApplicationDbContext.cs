using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Web_API.Models;
using Web_API.Models.DTO.Request;
using Web_API.Repository;
using Web_API.Repository.IRepository;
namespace Web_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserProductReview> UserProductReviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<ProductOptionVariant> ProductOptionVariantS { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().HasKey(u => u.Id);
            // builder.Entity<Role>().HasKey(r => r.Id);
            builder.Entity<Product>().HasKey(p => p.Id);
            builder.Entity<Category>().HasKey(c => c.Id);
            builder.Entity<Manufacturer>().HasKey(m => m.Id);
            // variants of a project
            builder.Entity<Option>().HasKey(o => o.Id);
            builder.Entity<ProductOption>().HasKey(po => po.Id);
            builder.Entity<ProductOptionVariant>().HasKey(pov => pov.Id);
            builder.Entity<RefreshToken>().HasKey(rt => rt.Id);

            // user with product relation: user product review relation N-M
            builder.Entity<UserProductReview>().HasKey(upr => upr.Id);
            builder.Entity<UserProductReview>().HasOne(upr => upr.User).WithMany(u => u.UserProductReviews).HasForeignKey(upr => upr.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<UserProductReview>().HasOne(upr => upr.Product).WithMany(p => p.UserProductReviews).HasForeignKey(upr => upr.ProductId).OnDelete(DeleteBehavior.Cascade);

            // Category Product Relation: 1-N
            builder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Cascade);

            // manufactor Product relation: 1-N: one category has a lot of manufactorer
            builder.Entity<Product>().HasOne(p => p.Manufacturer).WithMany(m => m.Products).HasForeignKey(m => m.ManufacturerId).OnDelete(DeleteBehavior.Cascade);

            // product and prodct variante relation ship: N-M
            builder.Entity<ProductVariant>().HasKey(pv => pv.Id);
            builder.Entity<ProductVariant>().HasOne(pv => pv.Product).WithMany(p => p.ProductVariants).HasForeignKey(pv => pv.ProductId).OnDelete(DeleteBehavior.Cascade);

            // relation fo product with options and variants
            builder.Entity<ProductOption>().HasOne(po => po.Product).WithMany(p => p.ProductOptions).HasForeignKey(po => po.ProductId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ProductOption>().HasOne(po => po.Option).WithMany(o => o.ProductOptions).HasForeignKey(po => po.OptionId).OnDelete(DeleteBehavior.NoAction);

            // ProductOptionVariant with ProductOption and prodctvariante relation ship: N-M
            builder.Entity<ProductOptionVariant>().HasOne(pov => pov.ProductVariant).WithMany(pv => pv.ProductOptionVariants).HasForeignKey(pov => pov.ProductVariantId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ProductOptionVariant>().HasOne(pov => pov.ProductOption).WithMany(po => po.ProductOptionVariants).HasForeignKey(pov => pov.ProductOptionId).OnDelete(DeleteBehavior.NoAction);

            // refresh token relation with the user
            builder.Entity<User>().HasOne(u => u.RefreshToken).WithOne(rt => rt.User).HasForeignKey<RefreshToken>(rt => rt.UserId);

            // builder.Entity<Category>().HasData(
            //     new { ID = "1", CategoryName = "Phone" }
            // );

            // builder.Entity<Option>().HasData(
            //     new { ID = "1", OptionName = "Ram/Memory" },
            //     new { ID = "2", OptionName = "Color" },
            //     new { ID = "3", OptionName = "Size" }
            // );

            // builder.Entity<Manufacturer>().HasData(
            //     new { ID = "1", OptionName = "Tecno" },
            //     new { ID = "2", OptionName = "Apple" },
            //     new { ID = "3", OptionName = "Samsung" },
            //     new { ID = "4", OptionName = "Huawei" }
            // );

            // builder.Entity<Product>().HasData(
            //     new
            //     {
            //         ID = "1",
            //         Title = "Tecno camon 17 pro",
            //         Description = "the best phone in its category",
            //         Specification = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets",
            //         Barcode = "102030",
            //         Discount = 0,
            //         ImageUrl = "",
            //         CategoryId = 1,
            //         ManufacturerId = 1,
            //         ProductVariantsVMs = new List<ProductVariantsVM>(){
            //                 new(){
            //                     Qty = 10,
            //                     Sku = "sku1",
            //                     Price = 210,
            //                     optionsValues = new Dictionary<int, string>() {
            //                         { 1, "8/128" },
            //                         { 2, "red" }
            //                     }
            //                 },
            //                  new(){
            //                     Qty = 7,
            //                     Sku = "sku2",
            //                     Price = 220,
            //                     optionsValues = new Dictionary<int, string>() {
            //                         { 1, "8/128" },
            //                         { 2, "blue" }
            //                     }
            //                 },
            //                 new(){
            //                     Qty = 3,
            //                     Sku = "sku2",
            //                     Price = 250,
            //                     optionsValues = new Dictionary<int, string>() {
            //                         { 1, "8/256" },
            //                         { 2, "blue" }
            //                     }
            //                 },
            //             }
            //     },
            //     new
            //     {
            //         ID = "2",
            //         Title = "IPhone 15",
            //         Description = "the best phone",
            //         Specification = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets",
            //         Barcode = "102031",
            //         Discount = 0,
            //         ImageUrl = "",
            //         CategoryId = 1,
            //         ManufacturerId = 2,
            //         ProductVariantsVMs = new List<ProductVariantsVM>(){
            //             new(){
            //                 Qty = 20,
            //                 Sku = "sku4",
            //                 Price = 1300,
            //                 optionsValues = new Dictionary<int, string>() {
            //                     { 1, "8/256" },
            //                     { 2, "silver" }
            //                 }
            //             },
            //                 new(){
            //                 Qty = 30,
            //                 Sku = "sku5",
            //                 Price = 1500,
            //                 optionsValues = new Dictionary<int, string>() {
            //                     { 1, "8/512" },
            //                     { 2, "tetanium" }
            //                 }
            //             },
            //             new(){
            //                 Qty = 3,
            //                 Sku = "sku6",
            //                 Price = 250,
            //                 optionsValues = new Dictionary<int, string>() {
            //                     { 1, "10/512" },
            //                     { 2, "gold" }
            //                 }
            //             },
            //         }
            //     }
            // );



        }

    }
}
