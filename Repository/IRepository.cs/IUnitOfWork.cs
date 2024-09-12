namespace Web_API.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductRepository Product { get; }
        IProductVariantRepository ProductVariant { get; }
        ICategoryRepository Category { get; }
        IManufacturerRepository Manufacturer { get; }
        IOptionRepository Option { get; }
        IProductOptionRepository ProductOption { get; }
        IProductOptionVariantRepository ProductOptionVariant { get; }
        IUserRepository User { get; }
        IUserProductReviewRepository UserProductReview { get; }
        IDashboardUserRepository DashboardUser { get; }
        IProductsRequestsRepository ProductsRequests { get; }
        void Save();
    }
}