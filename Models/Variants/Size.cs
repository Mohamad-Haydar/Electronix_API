using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models.Variants
{
    public class Size
    {
        public int Id { get; set; }
        public double SizeNumber { get; set; }
        [ValidateNever]
        public ICollection<PVSize> PVSizes { get; set; }
    }
}