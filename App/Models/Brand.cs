using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

[Table(("Brand"))]
[Index(nameof(IdBrand), IsUnique = true)]
[Index(nameof(BrandName), IsUnique = true)]
public partial class Brand
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("idBrand")]
    public int IdBrand { get; set; }

    [Column("brandName")]
    [MaxLength(64)]
    [Required]
    public string BrandName { get; set; } = null!;

    [InverseProperty(nameof(Product.NavigationBrand))]
    public virtual ICollection<Product> Products { get; set; } = null!;
}