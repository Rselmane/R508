using System.ComponentModel.DataAnnotations.Schema;

namespace App.DTO
{
    public class ProductAddDTO
    {
        public string? Nom { get; set; }
        public string? Description { get; set; }
        public string? NomPhoto { get; set; }
        public string? UriPhoto { get; set; }
        public int StockReel { get; set; }
        public int StockMin { get; set; }
        public int StockMax { get; set; }
        public string? Marque { get; set; }
        public string? TypeProduit { get; set; }


    }
}
