namespace App.DTO
{
    public class BrandDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        
        public override bool Equals(object? obj)
        {
            return obj is BrandDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }
}
