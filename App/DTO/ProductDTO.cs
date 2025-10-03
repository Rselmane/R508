namespace App.DTO;

public class ProductDTO
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Brand { get; set; }


    protected bool Equals(ProductDTO other)
    {
        return Name == other.Name && Type == other.Type && Brand == other.Brand;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ProductDTO)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Type, Brand);
    }
}
