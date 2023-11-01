using System.ComponentModel.DataAnnotations;

namespace Web_API.Models.DTO.Request
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}