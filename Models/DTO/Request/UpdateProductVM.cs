using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models.DTO.Request
{
    public class UpdateProductVM
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public string Barcode { get; set; }
        public double Discount { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }
        public ICollection<ProductVariantsVM> ProductVariantsVMs { get; set; }
    }
}