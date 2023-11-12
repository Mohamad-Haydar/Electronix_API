namespace Web_API.Models.DTO.Request
{
    public class CheckoutProductVM
    {
        public string ProductId { get; set; }
        public string ProductVariantId { get; set; }
        public int Qty { get; set; }
    }
}