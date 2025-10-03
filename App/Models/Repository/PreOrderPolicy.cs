using App.Models.Policies;

namespace App.Models.Repository
{
    public class PreOrderPolicy : IStockPolicy<Product>
    {
        public Rules CheckAvailability(Product product)
        {
            if(product.ActualStock< product.MinStock)
            {
                return Rules.Precommandable;
            }
            return Rules.Disponible;
        }
    }
}
