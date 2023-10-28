using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Web_API.Models;
using Web_API.Models.Variants;

namespace Web_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserProductReview> UserProductReviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<MemoryStorage> MemoryStorages { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<PVColor> PVColors { get; set; }
        public DbSet<PVSize> PVSizes { get; set; }
        public DbSet<PVMemoryStorage> PVMemoryStorages { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().HasKey(u => u.Id);
            builder.Entity<Role>().HasKey(r => r.Id);
            builder.Entity<Product>().HasKey(p => p.Id);
            builder.Entity<Category>().HasKey(c => c.Id);
            builder.Entity<Manufacturer>().HasKey(m => m.Id);
            // variants of a project
            builder.Entity<Color>().HasKey(c => c.Id);
            builder.Entity<MemoryStorage>().HasKey(ms => ms.Id);
            builder.Entity<Size>().HasKey(s => s.Id);

            builder.Entity<PVColor>().HasKey(c => c.Id);
            builder.Entity<PVMemoryStorage>().HasKey(ms => ms.Id);
            builder.Entity<PVSize>().HasKey(s => s.Id);



            // user with role relation:  user role relation N-M
            builder.Entity<UserRole>().HasKey(ur => ur.Id);
            builder.Entity<UserRole>().HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
            builder.Entity<UserRole>().HasOne(ur => ur.Role).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.RoleId);

            // User claim relation 1-N
            builder.Entity<UserClaim>().HasOne(uc => uc.User).WithMany(u => u.UserClaims).HasForeignKey(uc => uc.UserId);
            // Role claim relation 1-N
            builder.Entity<RoleClaim>().HasOne(rc => rc.Role).WithMany(r => r.RoleClaims).HasForeignKey(rc => rc.RoleId);

            // user with product relation: user product review relation N-M
            builder.Entity<UserProductReview>().HasKey(upr => upr.Id);
            builder.Entity<UserProductReview>().HasOne(upr => upr.User).WithMany(u => u.UserProductReviews).HasForeignKey(upr => upr.UserId);
            builder.Entity<UserProductReview>().HasOne(upr => upr.Product).WithMany(p => p.UserProductReviews).HasForeignKey(upr => upr.ProductId);

            // Category Product Relation: 1-N
            builder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);

            // manufactor Product relation: 1-N: one category has a lot of manufactorer
            builder.Entity<Product>().HasOne(p => p.Manufacturer).WithMany(m => m.Products).HasForeignKey(m => m.ManufacturerId);

            // product and prodct variante relation ship: N-M
            builder.Entity<ProductVariant>().HasKey(pv => pv.Id);
            builder.Entity<ProductVariant>().HasOne(pv => pv.Product).WithMany(p => p.ProductVariants).HasForeignKey(pv => pv.ProductId);

            builder.Entity<PVColor>().HasOne(pvc => pvc.ProductVariant).WithMany(pv => pv.PVColors).HasForeignKey(pvc => pvc.ProductVariantId);
            builder.Entity<PVMemoryStorage>().HasOne(pvc => pvc.ProductVariant).WithMany(pv => pv.PVMemoryStorages).HasForeignKey(pvc => pvc.ProductVariantId);
            builder.Entity<PVSize>().HasOne(pvc => pvc.ProductVariant).WithMany(pv => pv.PVSizes).HasForeignKey(pvc => pvc.ProductVariantId);

            builder.Entity<PVColor>().HasOne(pvc => pvc.Color).WithMany(pv => pv.PVColors).HasForeignKey(pvc => pvc.ColorId);
            builder.Entity<PVMemoryStorage>().HasOne(pvc => pvc.MemoryStorage).WithMany(pv => pv.PVMemoryStorages).HasForeignKey(pvc => pvc.MemoryStorageId);
            builder.Entity<PVSize>().HasOne(pvc => pvc.Size).WithMany(pv => pv.PVSizes).HasForeignKey(pvc => pvc.SizeId);
        }

    }
}
