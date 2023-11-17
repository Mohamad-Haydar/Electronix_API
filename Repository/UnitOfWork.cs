using Web_API.Data;
using Web_API.Repository.IRepository;

namespace Web_API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UnitOfWork> _logger;
        public IProductRepository Product { get; private set; }
        public IProductVariantRepository ProductVariant { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IManufacturerRepository Manufacturer { get; private set; }
        public IOptionRepository Option { get; private set; }
        public IProductOptionRepository ProductOption { get; private set; }
        public IProductOptionVariantRepository ProductOptionVariant { get; private set; }
        public IUserRepository User { get; private set; }
        public IUserProductReviewRepository UserProductReview { get; private set; }
        public UnitOfWork(ApplicationDbContext db, ILogger<UnitOfWork> logger)
        {
            _db = db;
            _logger = logger;
            Product = new ProductRepository(_db, _logger);
            ProductVariant = new ProductVariantRepository(_db, _logger);
            Category = new CategoryRepository(_db, _logger);
            Manufacturer = new ManufacturerRepository(_db, _logger);
            Option = new OptionRepository(_db, _logger);
            ProductOption = new ProductOptionRepository(_db, _logger);
            ProductOptionVariant = new ProductOptionVariantRepository(_db, _logger);
            User = new UserRepository(_db, _logger);
            UserProductReview = new UserProductReviewRepository(_db, _logger);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        // public static implicit operator UnitOfWork(Mock<IUnitOfWork> v)
        // {
        //     return v;
        // }
    }
}