using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FullDesc { get; set; } = string.Empty;
    public decimal Price { get; set; } 
    public decimal Discount { get; set; }
    public string ImageName { get; set; } = string.Empty;
    public int Qty { get; set; }  
    public string? Tags { get; set; } = string.Empty;
    public string? VideoUrl { get; set; } = string.Empty;

    
        
}