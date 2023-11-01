using System.ComponentModel.DataAnnotations;

namespace Web_API.Models.DTO.Request
{
    public class ReviewDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string ProductId { get; set; }
        [Required]
        public int Review { get; set; }
    }
}