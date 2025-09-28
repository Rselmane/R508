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
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.NavigationTypeProduct != null ? src.NavigationTypeProduct.TypeProductName : null))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.NavigationBrand != null ? src.NavigationBrand.BrandName : null))
                .ForMember(dest => dest.PhotoName, opt => opt.MapFrom(src => src.PhotoName))
                .ForMember(dest => dest.PhotoUri, opt => opt.MapFrom(src => src.PhotoUri))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.ActualStock))
                .ForMember(dest => dest.InRestocking, opt => opt.MapFrom(src => src.ActualStock <= src.MinStock));

        }
    }
}
