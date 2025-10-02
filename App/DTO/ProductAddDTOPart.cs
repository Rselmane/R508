


namespace App.DTO
{
    public partial class ProductAddDTO
    {
        public override bool Equals(object? obj)
        {
            return obj is ProductAddDTO dTO &&
                   Name == dTO.Name &&
                   Description == dTO.Description&&
                   PhotoName == dTO.PhotoName&&
                   PhotoUri == dTO.PhotoUri&&
                   RealStock == dTO.RealStock&&
                   MinStock == dTO.MinStock&&
                   MaxStock == dTO.MaxStock&&
                   Brand == dTO.Brand&&
                   Type == dTO.Type;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Name);
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
