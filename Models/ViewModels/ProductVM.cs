namespace Web_API.Models.ViewModels
{
    public class ProductVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public int Barcode { get; set; }
        public double Discount { get; set; }
        public string ImageUrl { get; set; }
        public int NummberOfReview { get; set; }
        public double Review { get; set; }
        public string Manufacturer { get; set; }
        public string Category { get; set; }
        public List<ProductVariantDetailVM> ProductVariantDetailVM { get; set; }
    }
}