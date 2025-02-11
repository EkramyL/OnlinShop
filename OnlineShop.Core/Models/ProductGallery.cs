namespace OnlineShop.Core.Models;

public class ProductGallery
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImageName { get; set; } = string.Empty;

    public Product? Product { get; set; }
}