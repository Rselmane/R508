namespace App.Models.Policies
{
    public enum Rules
    {
        Disponible,
        Indisponible,
        Precommandable
    }

    public interface IStockPolicy<TEntity> where TEntity : class
    {
        Rules CheckAvailability(Product product);
    }
 
}