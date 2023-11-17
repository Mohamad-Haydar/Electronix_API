using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models.DTO.Request
{
    public class AddProductVM
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        [Required]
        public string Barcode { get; set; }
        public double Discount { get; set; }
        public string ImageUrl { get; set; }
        public int NumberOfReview { get; set; }
        public double Review { get; set; }
        [ValidateNever]
        public int Star5 { get; set; }
        [ValidateNever]
        public int Star1 { get; set; }
        [ValidateNever]
        public int Star4 { get; set; }
        [ValidateNever]
        public int Star3 { get; set; }
        [ValidateNever]
        public int Star2 { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int ManufacturerId { get; set; }
        [Required]
        public ICollection<ProductVariantsVM> ProductVariantsVMs { get; set; }
    }
}