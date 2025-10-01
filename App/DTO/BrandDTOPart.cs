
namespace App.DTO
{
    public partial class BrandDTO
    {
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
