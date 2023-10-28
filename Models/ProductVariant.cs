using System.ComponentModel.DataAnnotations.Schema;
using Web_API.Models.Variants;

namespace Web_API.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string sku { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public ICollection<PVColor> PVColors { get; set; }
        public ICollection<PVMemoryStorage> PVMemoryStorages { get; set; }
        public ICollection<PVSize> PVSizes { get; set; }

    }
}