namespace App.Models.Repository
{
    public interface IProductRepository : IDataRepository<Product>
    {
        public Task<IEnumerable<Product>> GetAllWithRelationsAsync();

    }
}
