using System.ComponentModel.DataAnnotations.Schema;

namespace App.DTO
{
    public partial class ProductAddDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PhotoName { get; set; }
        public string? PhotoUri { get; set; }
        public int RealStock { get; set; }
        public int MinStock { get; set; }
        public int MaxStock { get; set; }
        public string? Brand { get; set; }
        public string? Type { get; set; }


    }
}