namespace App.DTO
{
    public class TypeProductUpdateDTO
    {   public string? Name { get; set; }
        
        
        public override bool Equals(object? obj)
        {
            return obj is TypeProductUpdateDTO dTO &&
                   Name == dTO.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
