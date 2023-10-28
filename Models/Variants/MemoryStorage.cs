using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models.Variants
{
    public class MemoryStorage
    {
        public int Id { get; set; }
        public int Memory { get; set; }
        public int Storage { get; set; }
        [ValidateNever]
        public ICollection<PVMemoryStorage> PVMemoryStorages { get; set; }
    }
}