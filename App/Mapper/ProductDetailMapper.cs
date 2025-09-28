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
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdProduct))
                .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.NavigationTypeProduct != null ? src.NavigationTypeProduct.TypeProductName : null))
                .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.NavigationBrand != null ? src.NavigationBrand.BrandName : null))
                .ForMember(dest => dest.Nomphoto, opt => opt.MapFrom(src => src.PhotoName))
                .ForMember(dest => dest.Uriphoto, opt => opt.MapFrom(src => src.PhotoUri))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.ActualStock))
                .ForMember(dest => dest.EnReappro, opt => opt.MapFrom(src => src.ActualStock <= src.MinStock));

        }
    }
}
