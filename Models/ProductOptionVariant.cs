using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Models
{
    public class ProductOptionVariant
    {
        [Key]
        public int Id { get; set; }
        public string ProductOptionId { get; set; }
        public string ProductVariantId { get; set; }
        public string Value { get; set; }

        [ForeignKey("ProductOptionId")]
        public ProductOption ProductOption { get; set; }

        [ForeignKey("ProductVariantId")]
        public ProductVariant ProductVariant { get; set; }
    }
}