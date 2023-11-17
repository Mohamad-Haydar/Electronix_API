using Web_API.Data;
using Web_API.Models;
using Web_API.Repository.IRepository;

namespace Web_API.Repository
{
    public class ProductOptionVariantRepository : Repository<ProductOptionVariant>, IProductOptionVariantRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;

        public ProductOptionVariantRepository(ApplicationDbContext db, ILogger logger) : base(db, logger)
        {
            _db = db;
            _logger = logger;
        }
        public override bool Remove(string id)
        {
            var pvs = dbSet.Where(x => x.ProductVariantId == id);
            dbSet.RemoveRange(pvs);
            return true;
        }
    }
}