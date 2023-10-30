
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models
{
    public class Option
    {
        [Key]
        public int Id { get; set; }
        public string OptionName { get; set; }
        [ValidateNever]
        public ICollection<ProductOption> ProductOptions { get; set; }
    }
}