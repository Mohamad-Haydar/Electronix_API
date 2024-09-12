using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Web_API.Models
{
    public class DashbordUser
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Adress { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}