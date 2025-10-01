
namespace App.Models
{
    public partial class TypeProduct
    {
        public override bool Equals(object? obj)
        {
            return obj is TypeProduct product &&
                   IdTypeProduct == product.IdTypeProduct &&
                   TypeProductName == product.TypeProductName &&
                   EqualityComparer<ICollection<Product>>.Default.Equals(Products, product.Products);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdTypeProduct, TypeProductName, Products);
        }
    }
}
