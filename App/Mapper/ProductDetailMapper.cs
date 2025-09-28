using App.DTO;
using App.Models;
using AutoMapper;


namespace App.Mapper;

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

        // ProduitDetailDto -> Produit
        CreateMap<ProductDetailDTO, Product>()
            .ForMember(dest => dest.IdProduct, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.NavigationTypeProduct, opt => opt.MapFrom(src =>
                src.Type != null ? new TypeProduct { TypeProductName = src.Type } : null))
            .ForMember(dest => dest.NavigationBrand, opt => opt.MapFrom(src =>
                src.Brand != null ? new Brand { BrandName = src.Name } : null))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.PhotoName, opt => opt.MapFrom(src => src.PhotoName))
            .ForMember(dest => dest.PhotoUri, opt => opt.MapFrom(src => src.PhotoUri))
            .ForMember(dest => dest.ActualStock, opt => opt.MapFrom(src => src.Stock ?? 0));
    }
}
