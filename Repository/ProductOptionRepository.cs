using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Web_API.Data;
using Web_API.Models;
using Web_API.Repository.IRepository;

namespace Web_API.Repository
{
    public class ProductOptionRepository : Repository<ProductOption>, IProductOptionRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;

        public ProductOptionRepository(ApplicationDbContext db, ILogger logger) : base(db, logger)
        {
            _db = db;
            _logger = logger;
        }
    }
}