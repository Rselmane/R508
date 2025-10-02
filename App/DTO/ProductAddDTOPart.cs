


namespace App.DTO
{
    public partial class ProductAddDTO
    {
        public override bool Equals(object? obj)
        {
            return obj is ProductAddDTO dTO &&
                   Name == dTO.Name &&
                   Description == dTO.Description &&
                   PhotoName == dTO.PhotoName &&
                   PhotoUri == dTO.PhotoUri &&
                   RealStock == dTO.RealStock &&
                   MinStock == dTO.MinStock &&
                   MaxStock == dTO.MaxStock &&
                   Brand == dTO.Brand &&
                   TypeProduct == dTO.TypeProduct;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Name);
            hash.Add(Description);
            hash.Add(PhotoName);
            hash.Add(PhotoUri);
            hash.Add(RealStock);
            hash.Add(MinStock);
            hash.Add(MaxStock);
            hash.Add(Brand);
            hash.Add(TypeProduct);
            return hash.ToHashCode();
        }
    }
}
