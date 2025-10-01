
namespace App.DTO
{
    public partial class ProductAddDTO
    {
        public override bool Equals(object? obj)
        {
            return obj is ProductAddDTO dTO &&
                   Nom == dTO.Nom &&
                   Description == dTO.Description &&
                   NomPhoto == dTO.NomPhoto &&
                   UriPhoto == dTO.UriPhoto &&
                   StockReel == dTO.StockReel &&
                   StockMin == dTO.StockMin &&
                   StockMax == dTO.StockMax &&
                   Marque == dTO.Marque &&
                   TypeProduit == dTO.TypeProduit;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Nom);
            hash.Add(Description);
            hash.Add(NomPhoto);
            hash.Add(UriPhoto);
            hash.Add(StockReel);
            hash.Add(StockMin);
            hash.Add(StockMax);
            hash.Add(Marque);
            hash.Add(TypeProduit);
            return hash.ToHashCode();
        }
    }
}
