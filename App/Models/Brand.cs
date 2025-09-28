using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

[Table(("Brand"))]
public class Brand
{
    [Key]
    [Column("idBrand")]
    public int IdBrand { get; set; }

    [Column("brandName")] public string BrandName { get; set; } = null!;

    [InverseProperty(nameof(Product.NavigationBrand))]
    public virtual ICollection<Product> Products { get; set; } = null!;
}