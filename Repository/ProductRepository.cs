using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Web_API.Data;
using Web_API.Models;
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

    }
}