using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models
{
    public class User : IdentityUser
    {
        public string Country { get; set; }
        public ICollection<UserProductReview> UserProductReviews { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}