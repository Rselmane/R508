namespace App.DTO
{

    public class ProductDetailDTO

    {
        public int Id { get; set; }


        public string? Name { get; set; }


        public string? Type { get; set; }


        public string? Brand { get; set; }


        public string? Description { get; set; }


        public string? PhotoName { get; set; }


        public string? PhotoUri { get; set; }


        public int? Stock { get; set; }


        public bool InRestocking { get; set; }

        
        public override bool Equals(object? obj)
        {
            return obj is ProductDetailDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   Type == dTO.Type &&
                   Brand == dTO.Brand &&
                   Description == dTO.Description &&
                   PhotoName == dTO.PhotoName &&
                   PhotoUri == dTO.PhotoUri &&
                   Stock == dTO.Stock &&
                   InRestocking == dTO.InRestocking;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Name);
            hash.Add(Type);
            hash.Add(Brand);
            hash.Add(Description);
            hash.Add(PhotoName);
            hash.Add(PhotoUri);
            hash.Add(Stock);
            hash.Add(InRestocking);
            return hash.ToHashCode();
        }
    }
}
