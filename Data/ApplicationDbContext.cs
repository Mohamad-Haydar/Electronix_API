using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_API.Models;
namespace Web_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

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
        }

    }
}
