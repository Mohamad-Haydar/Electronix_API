using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Models.Variants
{
    public class PVMemoryStorage
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        [ForeignKey("ProductVariantId")]
        public ProductVariant ProductVariant { get; set; }

        public int MemoryStorageId { get; set; }
        [ForeignKey("MemoryStorageId")]
        public MemoryStorage MemoryStorage { get; set; }
    }
}