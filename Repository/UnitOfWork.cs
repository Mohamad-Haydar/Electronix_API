using Web_API.Data;
using Web_API.Repository.IRepository;
using Web_API.Repository.VariantsRepository;

namespace Web_API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UnitOfWork> _logger;
        public IProductRepository Product { get; private set; }
        public ISizeRepository Size { get; private set; }
        public IColorRepository Color { get; private set; }
        public IMemoryStorageRepository MemoryStorage { get; private set; }
        public IPVSizeRepository PVSize { get; private set; }
        public IPVColorRepository PVColor { get; private set; }
        public IPVMemoryStorageRepository PVMemoryStorage { get; private set; }
        public IProductVariantRepository ProductVariant { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IManufacturerRepository Manufacturer { get; private set; }
        public UnitOfWork(ApplicationDbContext db, ILogger<UnitOfWork> logger)
        {
            _db = db;
            _logger = logger;
            Product = new ProductRepository(_db, _logger);
            ProductVariant = new ProductVariantRepository(_db, _logger);

            Size = new SizeRepository(_db, _logger);
            Color = new ColorRepository(_db, _logger);
            MemoryStorage = new MemoryStorageRepository(_db, _logger);
            PVSize = new PVSizeRepository(_db, _logger);
            PVColor = new PVColorRepository(_db, _logger);
            PVMemoryStorage = new PVMemoryStorageRepository(_db, _logger);
            Category = new CategoryRepository(_db, _logger);
            Manufacturer = new ManufacturerRepository(_db, _logger);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}