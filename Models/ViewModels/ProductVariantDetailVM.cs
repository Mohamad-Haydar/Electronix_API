using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models.ViewModels
{
    public class ProductVariantDetailVM
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
        public Dictionary<string, string> optionsValues { get; set; }
    }
}