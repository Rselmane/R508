using App.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Repository
{
    public class BrandManager(AppDbContext context) : CrudRepository<Brand>(context)
    {
        public override async Task<Brand?> GetByStringAsync(string name)
        {
            return await _context.Set<Brand>().FirstOrDefaultAsync(b => b.BrandName == name); 
        }
    }
}
