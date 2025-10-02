using App.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Repository;

public class ProductManager(AppDbContext context) : CrudRepository<Product>(context), IProductRepository
{

    public override async Task<Product?> GetByStringAsync(string name)
    {
        return await _context.Set<Product>() .FirstOrDefaultAsync(p => p.ProductName == name);
    }
    public async Task<IEnumerable<Product>> GetAllWithRelationsAsync()
    {
        return await _context.Set<Product>()
            .Include(p => p.NavigationTypeProduct)
            .Include(p => p.NavigationBrand)
            .ToListAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.NavigationBrand)
            .Include(p => p.NavigationTypeProduct)
            .FirstOrDefaultAsync(p => p.IdProduct == id);
    }



}
