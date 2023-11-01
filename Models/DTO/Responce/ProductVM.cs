namespace Web_API.Models.DTO.Responce
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
        public int Stars5 { get; set; }
        public int Stars4 { get; set; }
        public int Stars3 { get; set; }
        public int Stars2 { get; set; }
        public int Stars1 { get; set; }
        public string Manufacturer { get; set; }
        public string Category { get; set; }
        public List<ProductVariantDetailVM> ProductVariantDetailVM { get; set; }
    }
}