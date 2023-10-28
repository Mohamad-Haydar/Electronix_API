using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        [ValidateNever]
        public ICollection<Product> Products { get; set; }
    }
}