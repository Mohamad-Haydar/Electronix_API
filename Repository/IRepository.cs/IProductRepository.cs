using Web_API.Models;
using Web_API.Models.DTO.Request;

namespace Web_API.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        public Task<Product> GetProduct(string id);
        public Task AddProductRange(List<AddProductVM> products);
        public Task<IEnumerable<Product>> GetLatest(int number);
        public Task<IEnumerable<Product>> GetSpecialOffer(int number);
    }
}