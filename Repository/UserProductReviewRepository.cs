using Web_API.Data;
using Web_API.Models;
using Web_API.Repository.IRepository;

namespace Web_API.Repository
{
    public class UserProductReviewRepository : Repository<UserProductReview>, IUserProductReviewRepository
    {
        public UserProductReviewRepository(ApplicationDbContext db, ILogger logger) : base(db, logger)
        {
        }
    }
}