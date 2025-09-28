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
                .ForMember(dest => dest.Brand,
                           opt => opt.MapFrom(src => src.NavigationBrand != null ? src.NavigationBrand.BrandName : null))
                .ForMember(dest => dest.Type,
                           opt => opt.MapFrom(src => src.NavigationTypeProduct != null ? src.NavigationTypeProduct.TypeProductName : null));
        }
    }
}
