namespace Web_API.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductRepository Product { get; }
        ISizeRepository Size { get; }
        IColorRepository Color { get; }
        IMemoryStorageRepository MemoryStorage { get; }
        IProductVariantRepository ProductVariant { get; }
        IPVSizeRepository PVSize { get; }
        IPVColorRepository PVColor { get; }
        IPVMemoryStorageRepository PVMemoryStorage { get; }
        ICategoryRepository Category { get; }
        IManufacturerRepository Manufacturer { get; }
        void Save();
    }
}