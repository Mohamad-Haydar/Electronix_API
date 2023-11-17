using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models
{
    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }
        public string ManufacturerName { get; set; }
        [ValidateNever]
        public ICollection<Product> Products { get; set; }
    }
}