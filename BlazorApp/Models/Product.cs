namespace BlazorApp.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string? Description { get; set; }
    public string? PhotoName { get; set; }
    public string? PhotoUri { get; set; }
    public int? Stock { get; set; }
    public bool InRestocking { get; set; }
    public int RealStock { get; set; }
    public int MinStock { get; set; }
    public int MaxStock { get; set; }
}
