using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

[Table(("TypeProduct"))]
[Index(nameof(IdTypeProduct), IsUnique = true)]
[Index(nameof(TypeProductName), IsUnique = true)]
public partial class TypeProduct
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("idTypeProduct")]
    public int IdTypeProduct { get; set; }

    [Column("typeProductName")]
    [MaxLength(64)]
    [Required]
    public string TypeProductName { get; set; } = null!;
    
    [InverseProperty(nameof(Product.NavigationTypeProduct))]
    public virtual ICollection<Product> Products { get; set; } = null!;
}