using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

[Table(("produit"))]
public class Product
{
    [Key]
    [Column("idProduct")]
    public int IdProduct { get; set; }

    [Column("productName")] 
    public string ProductName { get; set; } = null!;

    [Column("description")] public string Description { get; set; } = null!;

    [Column("photoName")] public string PhotoName { get; set; } = null!;

    [Column("photoUri")] public string PhotoUri { get; set; } = null!;

    [Column("idTypeProduct")]
    public int? IdTypeProduct { get; set; }

    [Column("idBrand")]
    public int? IdBrand { get; set; }

    [Column("actualStock")]
    public int ActualStock { get; set; }
    
    [Column("minStock")]
    public int MinStock { get; set; }
    
    [Column("maxStock")]
    public int MaxStock { get; set; }

    [ForeignKey(nameof(IdBrand))]
    [InverseProperty(nameof(Brand.Products))]
    public virtual Brand? NavigationBrand { get; set; } = null!;
    
    [ForeignKey(nameof(IdTypeProduct))]
    [InverseProperty(nameof(TypeProduct.Products))]
    public virtual TypeProduct? NavigationTypeProduct { get; set; } = null!;

    protected bool Equals(Product other)
    {
        return ProductName == other.ProductName;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Product)obj);
    }
}