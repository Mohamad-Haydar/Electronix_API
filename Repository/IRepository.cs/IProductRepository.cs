using System.Linq.Expressions;
using Web_API.Models;
using Web_API.Models.DTO.Request;
using Web_API.Models.DTO.Responce;

namespace Web_API.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        public Task AddProductRange(List<AddProductVM> products);
        public Task<IEnumerable<Product>> GetLatest(int number, params Expression<Func<Product, object>>[] includes);
        public Task<IEnumerable<Product>> GetSpecialOffer(int number, params Expression<Func<Product, object>>[] includes);
    }
}