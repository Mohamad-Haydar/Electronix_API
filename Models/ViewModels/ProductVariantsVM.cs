using System.ComponentModel.DataAnnotations;

namespace Web_API.Models.ViewModels
{
    public class ProductVariantsVM
    {
        [Required]
        public int Qty { get; set; }
        public int Memory { get; set; }
        public int Storage { get; set; }
        public string Color { get; set; }
        public double Size { get; set; }
        [Required]
        public double Price { get; set; }
    }
}