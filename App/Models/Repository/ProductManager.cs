using App.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Repository;

public class ProductManager(AppDbContext context) : CrudRepository<Product>(context)
{

    public override async Task<Product?> GetByStringAsync(string nom)
    {
        return await _context.Set<Product>() .FirstOrDefaultAsync(p => p.ProductName == nom);
    }


}
