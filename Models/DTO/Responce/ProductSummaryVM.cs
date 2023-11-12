using System.ComponentModel.DataAnnotations;

namespace Web_API.Models.DTO.Responce
{
    public class ProductSummaryVM
    {
        [Key]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }
        // public string ImageUrl { get; set; }
        public int NummberOfReview { get; set; }
        public double Review { get; set; }

    }
}