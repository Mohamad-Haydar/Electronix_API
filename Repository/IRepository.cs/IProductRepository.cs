using Web_API.Models;
using Web_API.Models.DTO.Request;

namespace Web_API.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        public Task<Product> GetProduct(string id);
        public Task AddProductRange(List<AddProductVM> products);
    }
}