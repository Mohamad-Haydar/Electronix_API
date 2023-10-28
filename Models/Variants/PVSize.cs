using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Models.Variants
{
    public class PVSize
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        [ForeignKey("ProductVariantId")]
        public ProductVariant ProductVariant { get; set; }

        public int SizeId { get; set; }
        [ForeignKey("SizeId")]
        public Size Size { get; set; }
    }
}