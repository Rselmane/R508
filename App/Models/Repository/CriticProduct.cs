using App.Models.Policies;

namespace App.Models.Repository
{
    public class CriticProduct : IStockPolicy<Product>
    {
        public Rules CheckAvailability(Product product)
        {
            if(product.ActualStock< product.MinStock)
            {
                return Rules.Indisponible;
            }
            return Rules.Disponible;
        }
    }
}
