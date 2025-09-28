using App.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Repository;

public class TypeProductManager(AppDbContext context) : CrudRepository<TypeProduct>(context)
{
    public override async Task<TypeProduct?> GetByStringAsync(string name)
    {
        return await _context.Set<TypeProduct>().FirstOrDefaultAsync(p => p.TypeProductName == name);
    }
}
