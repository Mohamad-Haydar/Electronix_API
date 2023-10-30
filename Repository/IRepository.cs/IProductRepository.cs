using Web_API.Models;

namespace Web_API.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        public Task<Product> GetProduct(string id);
    }
}