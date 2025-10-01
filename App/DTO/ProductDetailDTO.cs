namespace App.DTO
{

    public partial class ProductDetailDTO

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

    }
}
