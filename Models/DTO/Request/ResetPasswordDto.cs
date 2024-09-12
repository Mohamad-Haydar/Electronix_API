using System.ComponentModel.DataAnnotations;

namespace Web_API.Models.DTO.Request
{
    public class ResetPasswordDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}