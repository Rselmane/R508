using App.DTO;
using App.Models;
using AutoMapper;

namespace App.Mapper
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            // ProduitDto <-> Produit
            CreateMap<ProductDTO, Product>()
                .ForMember(dest => dest.IdProduct, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.NavigationTypeProduct, opt => opt.MapFrom(src =>
                    src.Type != null ? new TypeProduct { TypeProductName = src.Type } : null))
                .ForMember(dest => dest.NavigationBrand, opt => opt.MapFrom(src =>
                    src.Brand != null ? new Brand { BrandName = src.Brand } : null))
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.PhotoName, opt => opt.Ignore())
                .ForMember(dest => dest.PhotoUri, opt => opt.Ignore())
                .ForMember(dest => dest.IdTypeProduct, opt => opt.Ignore())
                .ForMember(dest => dest.IdBrand, opt => opt.Ignore())
                .ForMember(dest => dest.ActualStock, opt => opt.Ignore())
                .ForMember(dest => dest.MinStock, opt => opt.Ignore())
                .ForMember(dest => dest.MaxStock, opt => opt.Ignore());

            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdProduct))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.NavigationTypeProduct.TypeProductName))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.NavigationBrand.BrandName));
        }
    }
}
