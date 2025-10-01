
namespace App.Models
{
    public partial class Product
    {
        public override bool Equals(object? obj)
        {
            return obj is Product product &&
                   IdProduct == product.IdProduct &&
                   ProductName == product.ProductName &&
                   Description == product.Description &&
                   PhotoName == product.PhotoName &&
                   PhotoUri == product.PhotoUri &&
                   IdTypeProduct == product.IdTypeProduct &&
                   IdBrand == product.IdBrand &&
                   ActualStock == product.ActualStock &&
                   MinStock == product.MinStock &&
                   MaxStock == product.MaxStock &&
                   EqualityComparer<Brand?>.Default.Equals(NavigationBrand, product.NavigationBrand) &&
                   EqualityComparer<TypeProduct?>.Default.Equals(NavigationTypeProduct, product.NavigationTypeProduct);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(IdProduct);
            hash.Add(ProductName);
            hash.Add(Description);
            hash.Add(PhotoName);
            hash.Add(PhotoUri);
            hash.Add(IdTypeProduct);
            hash.Add(IdBrand);
            hash.Add(ActualStock);
            hash.Add(MinStock);
            hash.Add(MaxStock);
            hash.Add(NavigationBrand);
            hash.Add(NavigationTypeProduct);
            return hash.ToHashCode();
        }
    }
}
