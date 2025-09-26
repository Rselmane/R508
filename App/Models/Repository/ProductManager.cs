using App.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Repository;

public class ProductManager(AppDbContext context) : CrudRepository<Produit>(context)
{

    public override async Task<Produit?> GetByStringAsync(string nom)
    {
        return await _context.Set<Produit>() .FirstOrDefaultAsync(p => p.NomProduit == nom);
    }


}
