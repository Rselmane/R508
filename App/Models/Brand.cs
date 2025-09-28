using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

public class Brand
{
    [Key]
    [Column("id_marque")]
    public int IdMarque { get; set; }

    [Column("nom_marque")] public string NomMarque { get; set; } = null!;

    [InverseProperty(nameof(Product.MarqueNavigation))]
    public virtual ICollection<Product> Produits { get; set; } = null!;
}