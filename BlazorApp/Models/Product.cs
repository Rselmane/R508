namespace BlazorApp.Models;

public class Product
{
    public int IdProduct { get; set; }
    public string ProductName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string PhotoName { get; set; } = null!;
    public string PhotoUri { get; set; } = null!;
}
