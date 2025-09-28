using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

public class TypeProduct
{
    [Key]
    [Column("idTypeProduct")]
    public int IdTypeProduct { get; set; }

    [Column("typeProductName")]
    public string TypeProductName { get; set; } = null!;
    
    [InverseProperty(nameof(Product.NavigationTypeProduct))]
    public virtual ICollection<Product> Products { get; set; } = null!;
}