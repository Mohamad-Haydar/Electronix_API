using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Models
{
    public class UserProductReview
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ProductId { get; set; }
        public int Review { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}