using System.ComponentModel.DataAnnotations.Schema;

namespace App.DTO
{
    public class ProductAddDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PhotoName { get; set; }
        public string? PhotoUri { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        public int MaxStock { get; set; }
        public string? Brand { get; set; }
        public string? Type { get; set; }
        
        public override bool Equals(object? obj)
        {
            return obj is ProductAddDTO dTO &&
                   Name == dTO.Name &&
                   Description == dTO.Description&&
                   PhotoName == dTO.PhotoName&&
                   PhotoUri == dTO.PhotoUri&&
                   Stock == dTO.Stock&&
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
            hash.Add(PhotoName);
            hash.Add(PhotoUri);
            hash.Add(Stock);
            hash.Add(MinStock);
            hash.Add(MaxStock);
            hash.Add(Brand);
            hash.Add(Type);
            return hash.ToHashCode();
        }
    }
}