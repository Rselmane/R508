using App.DTO;
using App.Models;
using AutoMapper;

namespace App.Mapper
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductAddDTO, Product>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.PhotoName, opt => opt.MapFrom(src => src.PhotoName))
                .ForMember(dest => dest.PhotoUri, opt => opt.MapFrom(src => src.PhotoUri))
                .ForMember(dest => dest.ActualStock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.MinStock, opt => opt.MapFrom(src => src.MinStock))
                .ForMember(dest => dest.MaxStock, opt => opt.MapFrom(src => src.MaxStock))
                .ForMember(dest => dest.NavigationBrand, opt => opt.MapFrom(src =>
                    src.Brand != null ? new Brand { BrandName = src.Brand } : null))
                .ForMember(dest => dest.NavigationTypeProduct, opt => opt.MapFrom(src =>
                    src.Type != null ? new TypeProduct { TypeProductName = src.Type } : null))
                .ForMember(dest => dest.IdProduct, opt => opt.Ignore())
                .ForMember(dest => dest.IdTypeProduct, opt => opt.Ignore())
                .ForMember(dest => dest.IdBrand, opt => opt.Ignore());

        }
    }

}
