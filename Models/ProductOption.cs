using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Web_API.Models
{
    public class ProductOption
    {
        [Key]
        public string Id { get; set; }
        public string ProductId { get; set; }
        public int? OptionId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [ForeignKey("OptionId")]
        public Option Option { get; set; }
        public ICollection<ProductOptionVariant> ProductOptionVariants { get; set; }
    }
}