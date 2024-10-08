using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models.DTO.Request
{
    public class ProductVariantsVM
    {
        [ValidateNever]
        public string Id { get; set; }
        [Required]
        public int Qty { get; set; }
        [Required]
        public string Sku { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public Dictionary<string, string> OptionsValues { get; set; }
    }
}