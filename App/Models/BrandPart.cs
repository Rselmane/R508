
namespace App.Models
{
    public partial class Brand
    {
        public override bool Equals(object? obj)
        {
            return obj is Brand brand &&
                   IdBrand == brand.IdBrand &&
                   BrandName == brand.BrandName &&
                   EqualityComparer<ICollection<Product>>.Default.Equals(Products, brand.Products);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdBrand, BrandName, Products);
        }
    }
}
