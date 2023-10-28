using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Models
{
    public class Product
    {
        [Key]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Barcode { get; set; }
        public double Discount { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string ConcurrencyStamp { get; set; }
        public int NummberOfReview { get; set; }
        public double Review { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public int ManufacturerId { get; set; }
        [ForeignKey("ManufacturerId")]
        public Manufacturer Manufacturer { get; set; }

        public ICollection<UserProductReview> UserProductReviews { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; }
    }
}