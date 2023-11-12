using System.ComponentModel.DataAnnotations;

namespace Web_API.Models.DTO.Request
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}