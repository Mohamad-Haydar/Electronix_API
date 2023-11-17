using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Web_API.Models.DTO.Request
{
    public class RegisterVM
    {
        [Required]
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Country { get; set; }

    }
}