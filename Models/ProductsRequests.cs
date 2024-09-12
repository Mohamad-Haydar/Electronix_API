using System.ComponentModel.DataAnnotations;

namespace Web_API.Models;


public class ProductsRequests
{
    [Key]
    public int Id { get; set; }
    public string PaymentSecret { get; set; }
    public string ImageUrl { get; set; }
    public string sku { get; set; }
    public int Qty { get; set; }
    public double TotalPrice { get; set; }
    public bool PaymentComplete { get; set; }
}