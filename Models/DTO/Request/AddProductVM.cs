using System.ComponentModel.DataAnnotations;

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
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int ManufacturerId { get; set; }
        [Required]
        public ICollection<ProductVariantsVM> ProductVariantsVMs { get; set; }
    }
}