using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models.Variants
{
    public class Color
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
        [ValidateNever]
        public ICollection<PVColor> PVColors { get; set; }
    }
}