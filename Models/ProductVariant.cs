using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Models
{
    public class ProductVariant
    {
        [Key]
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string sku { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public ICollection<ProductOptionVariant> ProductOptionVariants { get; set; }

    }
}