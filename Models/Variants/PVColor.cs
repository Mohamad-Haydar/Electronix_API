using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Models.Variants
{
    public class PVColor
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        [ForeignKey("ProductVariantId")]
        public ProductVariant ProductVariant { get; set; }

        public int ColorId { get; set; }
        [ForeignKey("ColorId")]
        public Color Color { get; set; }
    }
}