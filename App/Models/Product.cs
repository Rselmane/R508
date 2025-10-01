using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace App.Models;

[Table(("Product"))]
[Index(nameof(IdProduct), IsUnique = true)]
[Index(nameof(ProductName), IsUnique = true)]
[Index(nameof(IdTypeProduct))]
[Index(nameof(IdBrand))]
public partial class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("idProduct")]
    public int IdProduct { get; set; }

    [Column("productName")] 
    [MaxLength(128)]
    [Required]
    public string ProductName { get; set; } = null!;

    [Column("description")]
    [MaxLength(1024)]
    public string? Description { get; set; } = null;

    [Column("photoName")]
    [MaxLength(256)]
    public string? PhotoName { get; set; } = null;

    [Column("photoUri")]
    [MaxLength(512)]
    public string? PhotoUri { get; set; } = null;

    [Column("idTypeProduct")]
    public int? IdTypeProduct { get; set; } = null;

    [Column("idBrand")]
    public int? IdBrand { get; set; } = null;

    [Column("actualStock")]
    [Required]
    public int ActualStock { get; set; } = 0;

    [Column("minStock")]
    [Required]
    public int MinStock { get; set; } = 0;

    [Column("maxStock")]
    [Required]
    public int MaxStock { get; set; } = int.MaxValue;

    [ForeignKey(nameof(IdBrand))]
    [InverseProperty(nameof(Brand.Products))]
    public virtual Brand? NavigationBrand { get; set; } = null;
    
    [ForeignKey(nameof(IdTypeProduct))]
    [InverseProperty(nameof(TypeProduct.Products))]
    public virtual TypeProduct? NavigationTypeProduct { get; set; } = null;

}