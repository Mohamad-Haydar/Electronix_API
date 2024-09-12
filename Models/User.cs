using Microsoft.AspNetCore.Identity;

namespace Web_API.Models
{
    public class User : IdentityUser
    {
        public string Country { get; set; }
        public ICollection<UserProductReview> UserProductReviews { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public virtual DashbordUser DashbordUser { get; set; }
    }
}