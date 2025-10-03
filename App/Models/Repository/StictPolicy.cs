using App.Models.Policies;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Rewrite;

namespace App.Models.Repository
{
   
    public class StictRule : IStockPolicy<Product>
    {
        public Rules CheckAvailability(Product product)
        {
            if (product.MinStock <= product.ActualStock && product.ActualStock <= product.MinStock)
            {
                return Rules.Disponible;
            }
            return Rules.Indisponible;
        }
    }
}
