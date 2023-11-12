using Web_API.Models;
using Web_API.Models.DTO.Request;
using Web_API.Repository.IRepository;

namespace Web_API
{
    public class Seed
    {
        private readonly IUnitOfWork _unitOfWork;

        public Seed(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async void SeedDataContext()
        {
            // Add the categories
            if (!_unitOfWork.Category.Any())
            {
                var categories = new List<Category>()
                {
                    new(){
                        CategoryName = "Phone"
                    },
                };
                await _unitOfWork.Category.AddRange(categories);
            }

            // Add the options
            if (!_unitOfWork.Option.Any())
            {
                var options = new List<Option>()
                {
                    new(){
                        // Id = 1,
                        OptionName = "Ram/Memory"
                    },
                    new(){
                        // Id = 2,
                        OptionName = "Color"
                    },
                    new(){
                        // Id = 3,
                        OptionName = "Size"
                    },
                };
                await _unitOfWork.Option.AddRange(options);
            }

            // Add the Manufacturer
            if (!_unitOfWork.Manufacturer.Any())
            {
                var manufacturers = new List<Manufacturer>()
                {
                    new(){
                        // Id = 1,
                        ManufacturerName = "Tecno"
                    },
                    new(){
                        // Id = 2,
                        ManufacturerName = "Apple"
                    },
                    new(){
                        // Id = 3,
                        ManufacturerName = "Samsung"
                    },
                    new(){
                        // Id = 4,
                        ManufacturerName = "Huawei"
                    },
                };
                await _unitOfWork.Manufacturer.AddRange(manufacturers);
            }

            // Add the Products
            if (!_unitOfWork.Product.Any())
            {
                var products = new List<AddProductVM>()
                {
                    new() {
                        Title = "Tecno camon 17 pro",
                        Description = "the best phone in its category",
                        Specification = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets",
                        Barcode = "102030",
                        Discount = 0,
                        ImageUrl = "",
                        CategoryId = 1,
                        ManufacturerId = 1,
                        ProductVariantsVMs = new List<ProductVariantsVM>(){
                            new(){
                                Qty = 10,
                                Sku = "sku1",
                                Price = 210,
                                OptionsValues = new Dictionary<int, string>() {
                                    { 1, "8/128" },
                                    { 2, "red" }
                                }
                            },
                             new(){
                                Qty = 7,
                                Sku = "sku2",
                                Price = 220,
                                OptionsValues = new Dictionary<int, string>() {
                                    { 1, "8/128" },
                                    { 2, "blue" }
                                }
                            },
                            new(){
                                Qty = 3,
                                Sku = "sku2",
                                Price = 250,
                                OptionsValues = new Dictionary<int, string>() {
                                    { 1, "8/256" },
                                    { 2, "blue" }
                                }
                            },
                        }
                    },

                    new() {
                        Title = "IPhone 15",
                        Description = "the best phone",
                        Specification = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets",
                        Barcode = "102031",
                        Discount = 0,
                        ImageUrl = "",
                        CategoryId = 1,
                        ManufacturerId = 2,
                        ProductVariantsVMs = new List<ProductVariantsVM>(){
                            new(){
                                Qty = 20,
                                Sku = "sku4",
                                Price = 1300,
                                OptionsValues = new Dictionary<int, string>() {
                                    { 1, "8/256" },
                                    { 2, "silver" }
                                }
                            },
                             new(){
                                Qty = 30,
                                Sku = "sku5",
                                Price = 1500,
                                OptionsValues = new Dictionary<int, string>() {
                                    { 1, "8/512" },
                                    { 2, "tetanium" }
                                }
                            },
                            new(){
                                Qty = 3,
                                Sku = "sku6",
                                Price = 250,
                                OptionsValues = new Dictionary<int, string>() {
                                    { 1, "10/512" },
                                    { 2, "gold" }
                                }
                            },
                        }
                    },

                };

                await _unitOfWork.Product.AddProductRange(products);
            }

            _unitOfWork.Save();
        }
    }
}