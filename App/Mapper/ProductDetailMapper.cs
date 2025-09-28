using App.DTO;
using App.Models;
using AutoMapper;


namespace App.Mapper
{
    public class ProductDetailMapper : Profile
    {
        public ProductDetailMapper()
        
        {
            CreateMap<Product, ProductDetailDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdProduit))
                .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.NomProduit))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeProduitNavigation != null ? src.TypeProduitNavigation.NomTypeProduit : null))
                .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.MarqueNavigation != null ? src.MarqueNavigation.NomMarque : null))
                .ForMember(dest => dest.Nomphoto, opt => opt.MapFrom(src => src.NomPhoto))
                .ForMember(dest => dest.Uriphoto, opt => opt.MapFrom(src => src.UriPhoto))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockReel))
                .ForMember(dest => dest.EnReappro, opt => opt.MapFrom(src => src.StockReel <= src.StockMin));

        }
    }
}
