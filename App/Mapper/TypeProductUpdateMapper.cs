using App.DTO;
using App.Models;
using AutoMapper;

namespace App.Mapper;

public class TypeProductMapper : Profile
{
    public TypeProductMapper()
    {
        // TypeProduit -> TypeProduitDTO
        CreateMap<TypeProduct, TypeProductDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdTypeProduct))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TypeProductName));

        // TypeProduitDTO -> TypeProduit
        CreateMap<TypeProductDTO, TypeProduct>()
            .ForMember(dest => dest.IdTypeProduct, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TypeProductName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Products, opt => opt.Ignore()); // On ignore la navigation
    }
}
