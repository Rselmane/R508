using App.DTO;
using App.Models;
using AutoMapper;

namespace App.Mapper
{
    public class ProductMapper : Profile
    {
        public ProductMapper() 
        {
            
            // Mapping Produit -> ProduitDto
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Marque,
                           opt => opt.MapFrom(src => src.MarqueNavigation != null ? src.MarqueNavigation.NomMarque : null))
                .ForMember(dest => dest.Type,
                           opt => opt.MapFrom(src => src.TypeProduitNavigation != null ? src.TypeProduitNavigation.NomTypeProduit : null));
        }
    }
}
