using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Web_API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string Email { get; set; }
        public int EmailConfirmed { get; set; }
        public string HashedPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string ConcurrencyStamp { get; set; }
        public int TwoFactorEnabled { get; set; }
        public string refreshToken { get; set; }
        public DateTime Created_at { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<UserClaim> UserClaims { get; set; }
        public ICollection<UserProductReview> UserProductReviews { get; set; }
    }
}