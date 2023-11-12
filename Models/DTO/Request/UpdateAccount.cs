using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_API.Models.DTO.Request
{
    public class UpdateAccount
    {
        [ValidateNever]
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        [ValidateNever]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [ValidateNever]
        public string OldPassword { get; set; }
        [DataType(DataType.Password)]
        [ValidateNever]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [ValidateNever]
        public string ConfirmPassword { get; set; }
        [ValidateNever]
        public string PhoneNumber { get; set; }
        [ValidateNever]
        public string Address { get; set; }

    }
}