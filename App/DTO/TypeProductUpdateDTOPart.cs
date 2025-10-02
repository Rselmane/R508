
namespace App.DTO
{
    public partial class TypeProductUpdateDTO
    {
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