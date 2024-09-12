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
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public int NummberOfReview { get; set; }
        public double Review { get; set; }
        public DateTime AddedDate { get; set; }
    }
}