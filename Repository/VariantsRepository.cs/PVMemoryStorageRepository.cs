using Web_API.Data;
using Web_API.Models.Variants;
using Web_API.Repository.IRepository;

namespace Web_API.Repository.VariantsRepository
{
    public class PVMemoryStorageRepository : Repository<PVMemoryStorage>, IPVMemoryStorageRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;

        public PVMemoryStorageRepository(ApplicationDbContext db, ILogger logger) : base(db, logger)
        {
            _db = db;
            _logger = logger;
        }
    }
}